using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LinkableDoors
{
    static class LinkGroupUtility
    {
        public static void Notify_LinkableSpawned(ILinkData newObj)
        {
            ILinkGroup result = new LinkGroup(newObj);
            LinkGroupUtility.AttachGroupAround(newObj);
        }
        public static void Notify_LinkableDeSpawned(ILinkData delObj)
        {
            LinkGroupUtility.DetachGroupAround(delObj);
        }

        public static int Invert(int i) { return (i + 2) % 4; }

        public static void DetachGroupAround(ILinkData delObj)
        {
            ILinkGroup parent = delObj.GroupParent;
            if(parent.Children.Count() <= 1)
            {
                return;
            }
            if (parent.Children.First() == delObj || parent.Children.Last() == delObj)
            {
                parent.Remove(delObj);
                parent.RecalculateCenter();
            }
            else
            {
                parent.Split(delObj);
            }
            delObj.Reset();
            ILinkGroup result = new LinkGroup(delObj);
        }

        public static void AttachGroupAround(ILinkData newObj)
        {
            LinkGroupUtility.CheckAround(newObj, (i, current) =>
            {
                int invert = LinkGroupUtility.Invert(i);
                if (current.CanLinkFromOther(i) && newObj.CanLinkFromOther(invert))
                {
                    current.GroupParent.Concat(newObj.GroupParent);
                    newObj.Notify_Linked(current, i);
                    current.Notify_Linked(newObj, invert);
                }
            });
        }

        public static void RecalculateCenter(this ILinkGroup group)
        {
            group.TagGroup.Clear();
            if (!group.Any()) { return; }

            List<ILinkData> list = group.Children;
            list.Sort((x, y) => {
                int result = (int)(x.Pos.x - y.Pos.x);
                return result != 0 ? result : (int)(y.Pos.z - x.Pos.z);
            });

            int count = list.Count();
            int center = count / 2;
            int index = center;
            int mod2 = count % 2;

            for (int i = 0; i < center; i++)
            {
                list[i].DistFromCenter = index;
                list[i].PosTag = PositionTag.LeftSide;
                index--;
            }
            for (int i = center + mod2; i < count; i++)
            {
                index++;
                list[i].PosTag = PositionTag.RightSide;
                list[i].DistFromCenter = index;
            }
            if (mod2 == 0)
            {
                list[center - 1].PosTag |= PositionTag.LeftBorder;
                list[center].PosTag |= PositionTag.RightBorder;
            }
            else
            {
                list[center].PosTag = PositionTag.Center;
                list[center].DistFromCenter = 0;
            }
#if DEBUG
            Log.Message("*****RecalculateCenter(): children=" + list.Count);
            foreach(var a in list)
            {
                Log.Message("pos=" + a.DrawPos + " flag=" + a.PosTag + " dist=" + a.DistFromCenter);
            }
#endif
        }

        public static bool ShouldSingle(IntVec3 pos, Map map)
        {
            int num = LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.East, map);
            num += LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.West, map);
            if (num == 2)
            {
                return true;
            }

            num = LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.North, map);
            num += LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.South, map);
            if (num == 2)
            {
                return true;
            }
            return false;
        }

        delegate void Func(int i, ILinkData current);
        private static void CheckAround(ILinkData newObj, Func func)
        {
            for (int i = 0; i < 4; i++)
            {
                IntVec3 pos = newObj.Pos + GenAdj.CardinalDirections[i];
                foreach (var thing in newObj.Map.thingGrid.ThingsListAtFast(pos))
                {
                    Building_LinkableDoor door = thing as Building_LinkableDoor;
                    ILinkData current = door?.TryGetComp<CompLinkable>();
                    if (current != null)
                    {
                        func(i, current);
                    }
                }
            }
        }

        private static int AlignQualityAgainst(IntVec3 c, Map map)
        {
            if (!c.InBounds(map))
            {
                return 0;
            }
            if (!c.Walkable(map))
            {
                return 1;
            }
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i] as Blueprint;
                if (thing != null)
                {
                    if (thing.def.entityDefToBuild.passability == Traversability.Impassable)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }
    }
}

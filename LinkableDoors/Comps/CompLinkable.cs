using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    class CompLinkable : ThingComp, ILinkData
    {
        public ILinkGroup GroupParent { get; set; }
        public PositionTag PosTag { get; set; }
        public int DistFromCenter { get; set; } = 0;
        public float CommonField { get; set; } = 0;
        public bool Synchronize { get; set; } = false;
        public Rot4 LineDirection { get; private set; }
        public IntVec3 Pos => base.parent.Position;
        public Vector3 DrawPos => base.parent.DrawPos;
        public Map Map => base.parent.Map;
        public bool IsSingle => this.directLinks.Count() == 0;

        public Action<int> CallBack { get; set; }
        private Dictionary<ILinkData, int> directLinks = new Dictionary<ILinkData, int>();
        private CompProperties_Linkable compDef => (CompProperties_Linkable)base.props;
        
        public virtual bool CanLinkFromOther(int direction)
        {
            return this.linkableDirections(direction) && this.GroupParent.Children.Count() < this.compDef.linkableLimit && !LinkGroupUtility.ShouldSingle(this.Pos, this.Map);
        }
        public void Reset()
        {
            foreach (var a in this.directLinks)
            {
                a.Key.Notify_UnLinked(this);
            };
            this.directLinks.Clear();
            this.Synchronize = false;
            this.DistFromCenter = 0;
            this.PosTag = PositionTag.Center;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.LineDirection = default(Rot4);
            LinkGroupUtility.Notify_LinkableSpawned(this);
        }
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            LinkGroupUtility.Notify_LinkableDeSpawned(this);
        }
        public virtual void Notify_Linked(ILinkData other, int direction)
        {
            this.LineDirection = (direction == 0 || direction == 2) ? Rot4.East : Rot4.North;
            this.directLinks.Add(other, direction);
        }

        public virtual void Notify_UnLinked(ILinkData other)
        {
            this.directLinks.Remove(other);
        }
        private bool linkableDirections(int direction)
        {
            if (!this.directLinks.Any())
            {
                return true;
            }
            return this.directLinks.ContainsValue(direction);
        }
    }
}

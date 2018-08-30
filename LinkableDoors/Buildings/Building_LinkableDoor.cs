using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    public class Building_LinkableDoor : Building_Door
    {
        private const float ProtrudedDoorOffset = 0.05f;
        private ILinkData linkable;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.linkable = base.GetComp<CompLinkable>();
            if(this.linkable == null)
            {
                Log.Error("[LinkableDoors] This class does not have a component of Linkable.");
                return;
            }
            this.linkable.CallBack = this.CallBack;
        }
        public override void Tick()
        {
            base.Tick();
            if (base.Open)
            {
                foreach (var a in this.linkable.GroupParent.GetTagGroup(this.linkable.PosTag & (PositionTag.RightSide | PositionTag.LeftSide)))
                {
                    if (a.DistFromCenter < this.linkable.DistFromCenter)
                    {
                        a.CallBack(base.ticksUntilClose);
                        a.Synchronize = true;
                    }
                }
                if (this.linkable.Synchronize && base.ticksUntilClose <= 0)
                {
                    base.DoorTryClose();
                    this.linkable.Synchronize = false;
                }
            }
        }
        public void CallBack(int param)
        {
            if (!base.Open)
            {
                base.DoorOpen(param);
            }
            //base.FriendlyTouched();
            base.ticksUntilClose = param;
        }

        public override void Draw()
        {
            if (this.linkable.IsSingle)
            {
                base.Draw();
                return;
            }

            if (LinkGroupUtility.ShouldSingle(base.Position, base.Map))
            {
                LinkGroupUtility.DetachGroupAround(this.linkable);
                base.Draw();
                return;
            }

            base.Rotation = this.linkable.LineDirection;
            float num = Mathf.Clamp01((float)this.visualTicksOpen / (float)base.TicksToOpenNow);

            float[] move = { 0, 0 };
            int index;

            switch (this.linkable.PosTag)
            {
                case PositionTag.LeftSide:
                    this.linkable.CommonField = 1f * num;
                    move[0] = this.GetMoveOffset(PositionTag.LeftSide);
                    index = 0;
                    break;
                case PositionTag.RightSide:
                    this.linkable.CommonField = 1f * num;
                    move[0] = this.GetMoveOffset(PositionTag.RightSide);
                    index = 1;
                    break;
                case PositionTag.LeftBorder | PositionTag.LeftSide:
                    this.linkable.CommonField = (1f - ProtrudedDoorOffset) * num;
                    move[0] = this.GetMoveOffset(PositionTag.LeftSide);
                    move[1] = move[0];
                    index = 2;
                    break;
                case PositionTag.RightBorder | PositionTag.RightSide:
                    this.linkable.CommonField = (1f - ProtrudedDoorOffset) * num;
                    move[0] = this.GetMoveOffset(PositionTag.RightSide);
                    move[1] = move[0];
                    index = 3;
                    break;
                case PositionTag.Center:
                    this.linkable.CommonField = (0.5f - ProtrudedDoorOffset) * num;
                    move[0] = this.GetMoveOffset(PositionTag.LeftSide);
                    move[1] = this.GetMoveOffset(PositionTag.RightSide);
                    index = 4;
                    break;
                default:
                    Log.Error("[LinkableDoors] default");
                    index = 0;
                    break;
            }

            Vector3[] coefficients = LD_MeshPool.doorMeshPosSet[index].coefficients;
            Vector3[] offsets = LD_MeshPool.doorMeshPosSet[index].offsets;
            Mesh[] meshes = LD_MeshPool.doorMeshPosSet[index].meshes;

            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            Quaternion quat = rotation.AsQuat;

            for (int i = 0; i < 2; i++)
            {
                if (meshes[i] != null)
                {
                    Vector3 vector = this.DrawPos;
                    vector.y = Altitudes.AltitudeFor(AltitudeLayer.DoorMoveable) + offsets[i].y;
                    Vector3 vector2 = quat * coefficients[i];
                    Vector3 vector3 = quat * offsets[i];
                    vector += vector3 + vector2 * move[i];

                    Graphics.DrawMesh(meshes[i], vector, base.Rotation.AsQuat, this.Graphic.MatAt(base.Rotation, null), 0);
                }
            }
            base.Comps_PostDraw();
        }
        private float GetMoveOffset(PositionTag tag)
        {
            return this.linkable.GroupParent.GetCommonFieldSum(tag);
        }
    }
}

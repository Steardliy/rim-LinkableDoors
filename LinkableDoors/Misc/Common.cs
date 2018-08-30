using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    public interface ILinkGroup
    {
        List<ILinkData> Children { get; }
        Dictionary<PositionTag, List<ILinkData>> TagGroup { get; }

        IEnumerable<ILinkData> GetTagGroup(PositionTag tag);
        float GetCommonFieldSum(PositionTag tag);
        void SetCommonField(PositionTag tag, float value);

        void Concat(ILinkGroup other);
        void Split(ILinkData point);

        bool Any();
        void Remove(ILinkData delData);
        void Add(ILinkData newData);
    }
    public interface ILinkData
    {
        Action<int> CallBack { get; set; }

        ILinkGroup GroupParent { get; set; }
        PositionTag PosTag { get; set; }
        float CommonField { get; set; }
        bool Synchronize { get; set; }
        int DistFromCenter { get; set; }
        IntVec3 Pos { get; }
        Vector3 DrawPos { get; }
        Map Map { get; }
        bool IsSingle { get; }
        Rot4 LineDirection { get; }

        bool CanLinkFromOther(int direction);
        void Reset();

        void Notify_Linked(ILinkData other, int direction);
        void Notify_UnLinked(ILinkData other);
    }

    public enum PositionTag : int
    {
        None,
        RightSide = 1,
        LeftSide = 2,
        RightBorder = 4,
        LeftBorder = 8,
        Center = RightSide | LeftSide
    }
}

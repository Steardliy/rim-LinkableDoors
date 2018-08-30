using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LinkableDoors
{
    public class LinkGroup : ILinkGroup
    {
        private List<ILinkData> childrenInt = new List<ILinkData>();
        private Dictionary<PositionTag, List<ILinkData>> tagGroupInt = new Dictionary<PositionTag, List<ILinkData>>();
        
        public float GetCommonFieldSum(PositionTag tag)
        {
            float result = 0;
            foreach(var a in this.GetTagGroup(tag))
            {
                result += a.CommonField;
            }
            return result;
        }
        public void SetCommonField(PositionTag tag, float value)
        {
            foreach(var a in this.GetTagGroup(tag))
            {
                a.CommonField = value;
            }
        }

        public List<ILinkData> Children => this.childrenInt;
        public Dictionary<PositionTag, List<ILinkData>> TagGroup => this.tagGroupInt;

        public IEnumerable<ILinkData> GetTagGroup(PositionTag tag)
        {
            List<ILinkData> list;
            if (this.tagGroupInt.TryGetValue(tag, out list))
            {
                return list;
            }
            IEnumerable<ILinkData> result = this.childrenInt.SkipWhile(a => (a.PosTag & tag) == 0).TakeWhile(a => (a.PosTag & tag) != 0);
            this.tagGroupInt.Add(tag, result.ToList());
            return result;
        }

        public bool Any()
        {
            return this.childrenInt.Any();
        }
        public void Split(ILinkData point)
        {
            int index = this.childrenInt.IndexOf(point);
            ILinkGroup newGroup = new LinkGroup(this.childrenInt.Skip(index + 1));
            newGroup.RecalculateCenter();
            this.childrenInt.RemoveRange(index, this.childrenInt.Count - index);
            this.RecalculateCenter();
        }
        public void Concat(ILinkGroup other)
        {
            foreach(var a in other.Children)
            {
                this.Add(a);
            }
            this.RecalculateCenter();
        }
        public void Remove(ILinkData delData)
        {
            this.childrenInt.Remove(delData);
            delData.GroupParent = null;
        }
        public void Add(ILinkData newData)
        {
            this.childrenInt.Add(newData);
            newData.GroupParent = this;
        }

        public LinkGroup() { }
        public LinkGroup(ILinkData newData)
        {
            this.Add(newData);
            newData.PosTag = PositionTag.Center;
        }
        public LinkGroup(IEnumerable<ILinkData> newList)
        {
            this.childrenInt = newList.ToList();
            foreach (var a in newList)
            {
                a.GroupParent = this;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;

namespace Universal.Collections.Generic.Filters
{
    public abstract class VirtualFiltersList<T> : InfinityItemListBase<VirtualFilterItem<T>, T>
    {
        #region fields & properties
        #endregion fields & properties

        #region methods
        public List<VirtualFilterItem<T>> GetEnabledFilters() => ItemList.Items.Where(x => x.CanBeApplied).ToList();
        protected abstract IEnumerable<T> GetFilterData();
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(GetFilterData(), x => x);
        }
        #endregion methods
    }
}
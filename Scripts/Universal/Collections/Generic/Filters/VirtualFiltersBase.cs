using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Universal.Collections.Generic.Filters
{
    [System.Serializable]
    public abstract class VirtualFiltersBase<T>
    {
        #region fields & properties
        protected bool IsMainFilterApplied => mainFilter == null || mainFilter.CanBeApplied;
        [SerializeField] private VirtualFilter mainFilter;
        #endregion fields & properties

        #region methods
        protected IEnumerable<T> IterateOverSmartFilters<SmartFilterType>(IEnumerable<Component> components, System.Func<T, SmartFilterType> getItem, IEnumerable<T> newList)
        {
            foreach (ISmartFilter<SmartFilterType> el in components.Cast<ISmartFilter<SmartFilterType>>())
            {
                if (!el.VirtualFilter.CanBeApplied) continue;
                el.UpdateFilterData();
                newList = newList.Where(x => el.FilterItem(getItem.Invoke(x)));
            }
            return newList;
        }
        protected virtual IEnumerable<T> ApplyMainFilters(IEnumerable<T> newList)
        {
            return newList;
        }
        public List<T> ApplyFilters(IEnumerable<T> originalItems)
        {
            return ApplyMainFilters(originalItems).ToList();
        }
        #endregion methods
    }
}
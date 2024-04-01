using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Collections.Generic.Filters
{
    public abstract class VirtualDropdownFilter<T> : VirtualFilter
    {
        #region fields & properties
        [SerializeField] private VirtualFiltersList<T> filters;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            ChangeFiltersVisibility(CanBeApplied);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        private void ChangeFiltersVisibility(bool visible) => filters.gameObject.SetActive(visible);
        public List<VirtualFilterItem<T>> GetEnabledFilters() => filters.GetEnabledFilters();
        #endregion methods
    }
}
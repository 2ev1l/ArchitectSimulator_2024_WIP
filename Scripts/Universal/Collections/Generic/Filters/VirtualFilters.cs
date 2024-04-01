using DebugStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Collections.Generic.Filters
{
    /// <summary>
    /// Constructor required!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="SmartFilterType"></typeparam>
    [System.Serializable]
    public class VirtualFilters<T, SmartFilterType> : VirtualFiltersBase<T>
    {
        #region fields & properties
        [SerializeField] private VirtualFilter[] filters;
        [SerializeField] private bool ignoreMainFilter = false;
        private System.Func<T, SmartFilterType> GetItem;
        #endregion fields & properties

        #region methods
        protected override IEnumerable<T> ApplyMainFilters(IEnumerable<T> newList)
        {
            if (!ignoreMainFilter)
            {
                if (!IsMainFilterApplied) 
                    return base.ApplyMainFilters(newList);
            }
            newList = IterateOverSmartFilters<SmartFilterType>(filters, GetItem, newList);

            return base.ApplyMainFilters(newList);
        }
        /// <summary>
        /// Invoke this in control class for check
        /// </summary>
        public virtual void Validate(Component context)
        {
            DebugCommands.CastInterfacesList<ISmartFilter<SmartFilterType>>(filters, nameof(filters), context);
        }

        public VirtualFilters(Func<T, SmartFilterType> getItem)
        {
            GetItem = getItem;
        }
        #endregion methods
    }
}
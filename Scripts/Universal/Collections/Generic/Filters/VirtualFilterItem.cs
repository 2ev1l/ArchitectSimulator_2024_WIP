using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Universal.Collections.Generic.Filters
{
    public abstract class VirtualFilterItem<T> : VirtualFilter, IListUpdater<T>
    {
        #region fields & properties
        public T Value => value;
        private T value;
        #endregion fields & properties

        #region methods
        public void OnListUpdate(T param)
        {
            value = param;
            UpdateUI();
        }
        protected abstract void UpdateUI();
        #endregion methods
    }
}
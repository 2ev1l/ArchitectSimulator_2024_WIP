using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Collections.Generic.Filters
{
    public abstract class VirtualDataFilter<T> : VirtualFilter
    {
        #region fields & properties
        public abstract T Data { get; }
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
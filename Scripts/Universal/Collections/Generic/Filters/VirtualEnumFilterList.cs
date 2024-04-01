using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Collections.Generic.Filters
{
    public abstract class VirtualEnumFilterList<T> : VirtualFiltersList<T> where T : System.Enum
    {
        #region fields & properties
        protected IReadOnlyList<T> EnumValues
        {
            get
            {
                if (enumValues == null)
                {
                    enumValues = new List<T>();
                    foreach (T el in Enum.GetValues(typeof(T)))
                    {
                        enumValues.Add(el);
                    }
                }
                return enumValues;
            }
        }
        private List<T> enumValues = null;
        #endregion fields & properties

        #region methods
        protected override IEnumerable<T> GetFilterData() => EnumValues;
        #endregion methods
    }
}
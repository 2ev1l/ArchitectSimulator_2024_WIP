using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer
{
    public abstract class TextFilterItem<T> : VirtualFilterItem<T>
    {
        #region fields & properties
        protected TextMeshProUGUI Text => text;
        [SerializeField] private TextMeshProUGUI text;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
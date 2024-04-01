using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer
{
    public abstract class ImageFilterItem<T> : VirtualFilterItem<T>
    {
        #region fields & properties
        protected Image Image => image;
        [SerializeField] private Image image;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
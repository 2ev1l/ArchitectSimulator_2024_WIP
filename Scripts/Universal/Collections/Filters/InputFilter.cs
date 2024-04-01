using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer
{
    public abstract class InputFilter<T> : VirtualDataFilter<T>
    {
        #region fields & properties
        protected TMP_InputField InputField => inputField;
        [SerializeField] private TMP_InputField inputField;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
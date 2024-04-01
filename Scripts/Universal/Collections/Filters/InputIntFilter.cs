using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer
{
    public class InputIntFilter : InputFilter<int>
    {
        #region fields & properties
        public override int Data => System.Convert.ToInt32(InputField.text);
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
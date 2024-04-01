using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer
{
    public class ConstructionResourceTypeFilterItem : TextFilterItem<ConstructionType>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            Text.text = Value.ToLanguage();
        }
        #endregion methods
    }
}
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer
{
    public class ResourceColorFilterItem : ImageFilterItem<ResourceColor>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            Image.color = Value.ToColorRGB();
        }
        #endregion methods
    }
}
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Collections
{
    public class WarehouseItem : PremiseItem
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI maxSpaceText;
        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            base.UpdateUI();
            WarehouseInfo info = (WarehouseInfo)Context;
            maxSpaceText.text = $"{info.Space:F2} m3";
        }
        #endregion methods
    }
}
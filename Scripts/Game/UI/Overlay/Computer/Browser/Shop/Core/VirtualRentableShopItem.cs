using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class VirtualRentableShopItem<T> : VirtualShopItem<T> where T : RentableShopItemData, ICloneable<T>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI rentPriceText;
        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            base.UpdateUI();
            rentPriceText.text = $"${Context.ItemData.Item.RentPrice}";
        }
        #endregion methods
    }
}
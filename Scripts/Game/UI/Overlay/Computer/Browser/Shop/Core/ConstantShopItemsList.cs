using Game.Serialization.World;
using Game.UI.Overlay.Computer.Browser.Shop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser
{
    /// <summary>
    /// Use this class if your data are updated rarely
    /// </summary>
    public class ConstantShopItemsList<T> : VirtualShopItemsList<T> where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        private List<T> lastRenderedItemsData = new();
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            Shop.Data.OnDataChanged += CheckRenderedData;
            CheckRenderedData();
        }
        protected override void OnDisable()
        {
            Shop.Data.OnDataChanged -= CheckRenderedData;
        }
        private void CheckRenderedData()
        {
            int itemsCount = 0;
            foreach (var el in lastRenderedItemsData)
            {
                try
                {
                    if (lastRenderedItemsData[itemsCount] != Shop.Data.Items[itemsCount])
                    {
                        UpdateShop();
                        return;
                    }
                }
                catch
                {
                    UpdateShop();
                    return;
                }
                itemsCount++;
            }
            if (itemsCount != Shop.Data.Items.Count)
                UpdateShop();
        }
        private void UpdateShop()
        {
            UpdateListData();
            lastRenderedItemsData = Shop.Data.Items.ToList();
        }
        #endregion methods
    }
}
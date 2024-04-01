using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    [System.Serializable]
    public class VirtualShopItemContext<T> where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        public ShopData<T> ShopData { get; }
        public CountableItem<T> ItemData { get; set; }
        #endregion fields & properties

        #region methods
        public VirtualShopItemContext(ShopData<T> shopData, T itemData)
        {
            ShopData = shopData;
            ItemData = new(itemData, 1);
        }
        public VirtualShopItemContext(ShopData<T> shopData, CountableItem<T> itemData)
        {
            ShopData = shopData;
            ItemData = itemData;
        }
        #endregion methods
    }
}
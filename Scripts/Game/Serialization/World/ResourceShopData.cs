using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;
using Universal.Collections.Generic;
using System.Linq;
using System;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class ResourceShopData : ShopData<ResourceShopItemData>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override bool CartItemsPredicate(CountableItem<ResourceShopItemData> existCartItem, ResourceShopItemData currentItem)
        {
            return existCartItem.Item.ResourceType == currentItem.ResourceType && base.CartItemsPredicate(existCartItem, currentItem);
        }
        public override bool CanPurchaseCart()
        {
            float resourcesVolume = Cart.Items.Sum(x => x.Item.Info.ResourceInfo.Prefab.VolumeM3 * x.Count);
            if (!GameData.Data.CompanyData.WarehouseData.CanAddResource(resourcesVolume)) return false;
            return base.CanPurchaseCart();
        }
        protected override List<ResourceShopItemData> GetNewData()
        {
            List<ResourceShopItemData> result = new();
            ResourceShopItemData item = null;
            foreach (var el in DB.Instance.BuyableConstructionResourceInfo.Data)
            {
                item = new(el.Data.ResourceInfo.Id, el.Data.Price, CustomMath.GetRandomChance(95) ? 0 : el.Data.GetRandomDiscount(), ResourceType.Construction);
                result.Add(item);
            }
            //todo with other resource types

            return result;
        }
        #endregion methods
    }
}
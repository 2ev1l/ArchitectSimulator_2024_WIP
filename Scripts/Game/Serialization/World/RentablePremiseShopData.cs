using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class RentablePremiseShopData : ShopData<RentablePremiseShopItemData>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override bool CartItemsPredicate(CountableItem<RentablePremiseShopItemData> existCartItem, RentablePremiseShopItemData currentItem)
        {
            return existCartItem.Item.PremiseType == currentItem.PremiseType && base.CartItemsPredicate(existCartItem, currentItem);
        }
        protected override List<RentablePremiseShopItemData> GetNewData()
        {
            List<RentablePremiseShopItemData> result = new();
            RentablePremiseShopItemData item = null;
            foreach (var el in DB.Instance.RentableOfficeInfo.Data)
            {
                item = new(el.Data.PremiseInfo.Id, el.Data.Price, CustomMath.GetRandomChance(85) ? 0 : el.Data.GetRandomDiscount(), RentablePremiseType.Office);
                result.Add(item);
            }
            foreach (var el in DB.Instance.RentableWarehouseInfo.Data)
            {
                item = new(el.Data.PremiseInfo.Id, el.Data.Price, CustomMath.GetRandomChance(85) ? 0 : el.Data.GetRandomDiscount(), RentablePremiseType.Warehouse);
                result.Add(item);
            }

            //todo with plot

            return result;
        }
        #endregion methods
    }
}
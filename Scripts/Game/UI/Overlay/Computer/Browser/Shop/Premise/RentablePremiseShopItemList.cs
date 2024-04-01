using Game.Serialization.World;
using Game.UI.Overlay.Computer.Browser;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class RentablePremiseShopItemList : ConstantShopItemsList<RentablePremiseShopItemData>
    {
        #region fields & properties
        [SerializeField] private RentablePremiseType updateableType = RentablePremiseType.Office;
        #endregion fields & properties

        #region methods
        protected override IEnumerable<VirtualShopItemContext<RentablePremiseShopItemData>> GetFilteredItems(IEnumerable<VirtualShopItemContext<RentablePremiseShopItemData>> currentItems)
        {
            currentItems = currentItems.Where(x => x.ItemData.Item.PremiseType == updateableType);
            return base.GetFilteredItems(currentItems);
        }
        #endregion methods
    }
}
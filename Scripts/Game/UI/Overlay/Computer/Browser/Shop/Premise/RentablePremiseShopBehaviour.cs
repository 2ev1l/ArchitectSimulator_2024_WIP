using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class RentablePremiseShopBehaviour : VirtualShopBehaviour<RentablePremiseShopItemData>
    {
        #region fields & properties
        public override ShopData<RentablePremiseShopItemData> Data => GameData.Data.BrowserData.PremiseShop;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
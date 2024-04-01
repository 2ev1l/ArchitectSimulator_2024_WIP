using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ResourceShopBehaviour : VirtualShopBehaviour<ResourceShopItemData>
    {
        #region fields & properties
        public override ShopData<ResourceShopItemData> Data => GameData.Data.BrowserData.ResourceShop;
        #endregion fields & properties

        #region methods
        
        #endregion methods
    }
}
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ShopItemDiscountFilter : VirtualFilter, ISmartFilter<ShopItemData>
    {
        #region fields & properties
        public VirtualFilter VirtualFilter => this;
        #endregion fields & properties

        #region methods
        public void UpdateFilterData()
        {
            
        }
        public bool FilterItem(ShopItemData item)
        {
            return item.HasDiscount;
        }
        #endregion methods
    }
}
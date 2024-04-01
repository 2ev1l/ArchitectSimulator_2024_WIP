using Game.Serialization.World;
using Game.UI.Overlay.Computer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ResourceShopCartItemList : VirtualShopCartItemsList<ResourceShopItemData>
    {
        #region fields & properties
        [SerializeField] private ResourceType updatableType = ResourceType.Construction;
        #endregion fields & properties

        #region methods
        protected override bool BaseFilterForItem(CountableItem<ResourceShopItemData> item)
        {
            return item.Item.ResourceType == updatableType;
        }
        public override void UpdateListData()
        {
            base.UpdateListData();
        }
        #endregion methods
    }
}
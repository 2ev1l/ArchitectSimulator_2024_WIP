using DebugStuff;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ResourceShopItemList : ConstantShopItemsList<ResourceShopItemData>
    {
        #region fields & properties
        [SerializeField] private ResourceType updateableType = ResourceType.Construction;
        [SerializeField] private VirtualFilters<VirtualShopItemContext<ResourceShopItemData>, ResourceInfo> resourceFilters = new(x => x.ItemData.Item.Info.ResourceInfo);
        #endregion fields & properties

        #region methods
        protected override IEnumerable<VirtualShopItemContext<ResourceShopItemData>> GetFilteredItems(IEnumerable<VirtualShopItemContext<ResourceShopItemData>> currentItems)
        {
            currentItems = currentItems.Where(x => x.ItemData.Item.ResourceType == updateableType);
            currentItems = resourceFilters.ApplyFilters(currentItems);
            return base.GetFilteredItems(currentItems);
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            resourceFilters.Validate(this);
        }
        #endregion methods

    }
}
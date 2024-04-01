using EditorCustom.Attributes;
using Game.DataBase;
using Game.Serialization.World;
using Game.UI.Overlay.Computer.Collections;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ResourceShopItem : VirtualShopItem<ResourceShopItemData>
    {
        #region fields & properties
        public ResourceInfo ResourceInfo => Context.ItemData.Item.Info.ResourceInfo;
        [Title("Resource")]
        [SerializeField] private ResourceItem resourceItem;
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(VirtualShopItemContext<ResourceShopItemData> param)
        {
            base.OnListUpdate(param);
            resourceItem.OnListUpdate(ResourceInfo);
        }
        #endregion methods
    }
}
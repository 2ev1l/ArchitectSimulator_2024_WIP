using EditorCustom.Attributes;
using Game.Serialization.World;
using Game.UI.Overlay.Computer.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic.Filters;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    /// <summary>
    /// Require controlled optimization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VirtualShopItemsList<T> : InfinityFilteredItemListBase<VirtualShopItem<T>, VirtualShopItemContext<T>> where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        protected VirtualShopBehaviour<T> Shop => shop;
        [SerializeField] private VirtualShopBehaviour<T> shop;
        [SerializeField] private VirtualFilters<VirtualShopItemContext<T>, ShopItemData> shopItemDataFilters = new(x => x.ItemData.Item);
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            shop.Data.OnDataChanged += UpdateListData;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            shop.Data.OnDataChanged -= UpdateListData;
            base.OnDisable();
        }
        protected override IEnumerable<VirtualShopItemContext<T>> GetFilteredItems(IEnumerable<VirtualShopItemContext<T>> currentItems)
        {
            currentItems = shopItemDataFilters.ApplyFilters(currentItems);
            return base.GetFilteredItems(currentItems);
        }
        protected override void UpdateCurrentItems(List<VirtualShopItemContext<T>> currentItemsReference)
        {
            currentItemsReference.Clear();
            foreach (T el in shop.Data.Items)
            {
                currentItemsReference.Add(new(shop.Data, el));
            }
        }
        protected virtual void OnValidate()
        {
            shopItemDataFilters.Validate(this);
        }
        #endregion methods

    }
}
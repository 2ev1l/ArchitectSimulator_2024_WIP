using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public abstract class VirtualShopCartItemsList<T> : InfinityItemListBase<VirtualShopItem<T>, VirtualShopItemContext<T>> where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        private List<VirtualShopItemContext<T>> CartList
        {
            get
            {
                cartList.Clear();
                int totalCount = shop.Data.Cart.Items.Count;
                for (int i = 0; i < totalCount; ++i)
                {
                    CountableItem<T> item = shop.Data.Cart.Items[i];
                    if (!BaseFilterForItem(item)) continue;
                    cartList.Add(new(shop.Data, item));
                }
                return cartList;
            }
        }
        private List<VirtualShopItemContext<T>> cartList = new();
        [SerializeField] private VirtualShopBehaviour<T> shop;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            shop.Data.Cart.OnItemRemoved += UpdateListData;
            shop.Data.Cart.OnItemAdded += UpdateListData;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            shop.Data.Cart.OnItemRemoved -= UpdateListData;
            shop.Data.Cart.OnItemAdded -= UpdateListData;
            base.OnDisable();
        }
        protected abstract bool BaseFilterForItem(CountableItem<T> item);
        private void UpdateListData(CountableItem<T> _) => UpdateListData();
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(CartList, x => x);
        }
        #endregion methods
    }
}
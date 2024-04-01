using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.Serialization.World
{
    [System.Serializable]
    public abstract class ShopData<T> where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        public UnityAction OnDataChanged;
        public IReadOnlyList<T> Items => items;
        [SerializeField] protected List<T> items = new();

        /// <summary>
        /// Don't modify items. Use <see cref="AddToCart(T, int)"/>, <see cref="RemoveFromCart(T, int)"/>
        /// </summary>
        public CountableItemList<T> Cart => cart;
        [SerializeField] protected CountableItemList<T> cart = new();
        #endregion fields & properties

        #region methods
        public void AddToCart(T shopItem, int count)
        {
            if (count == 0) return;
            T clone = shopItem.Clone();
            Cart.AddItem(clone, x => CartItemsPredicate(x, clone), count);
        }
        public void RemoveFromCart(T cartItem, int count)
        {
            if (count == 0) return;
            Cart.RemoveItem(x => CartItemsPredicate(x, cartItem), ref count);
        }
        public virtual bool CanPurchaseCart()
        {
            int moneySum = cart.Items.Sum(x => x.Item.FinalPrice * x.Count);
            return GameData.Data.PlayerData.Wallet.CanDecreaseValue(moneySum);
        }
        /// <summary>
        /// For default, compares only for id
        /// </summary>
        /// <param name="existCartItem"></param>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        protected virtual bool CartItemsPredicate(CountableItem<T> existCartItem, T currentItem)
        {
            return existCartItem.Item.Id == currentItem.Id;
        }
        /// <summary>
        /// You need to check manually for <see cref="CanPurchaseCart"/> <br></br>
        /// Clears all cart items
        /// </summary>
        public virtual void PurchaseCart()
        {
            foreach (var el in cart.Items)
            {
                el.Item.OnPurchase(el.Count);
            }
            cart.Clear();
        }
        public void GenerateNewData()
        {
            cart.Clear();
            items = GetNewData();
            OnDataChanged?.Invoke();
        }
        protected abstract List<T> GetNewData();
        #endregion methods
    }
}
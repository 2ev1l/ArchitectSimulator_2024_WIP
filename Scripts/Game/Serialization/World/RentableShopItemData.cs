using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public abstract class RentableShopItemData : ShopItemData
    {
        #region fields & properties
        public int RentPrice => rentPrice;
        [SerializeField][Min(1)] private int rentPrice = 1;
        #endregion fields & properties

        #region methods
        protected RentableShopItemData(int id, int startPrice, int discount, int rentPrice) : base(id, startPrice, discount)
        {
            this.rentPrice = rentPrice;
        }
        #endregion methods

    }
}
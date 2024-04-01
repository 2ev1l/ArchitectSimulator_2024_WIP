using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    public abstract class BuyableObjectItemData<T> : ShopItemData where T : BuyableObject
    {
        #region fields & properties
        ///<exception cref="System.NullReferenceException"></exception>
        public T Info
        {
            get
            {
                if (info == null)
                {
                    try { info = GetInfo(); }
                    catch { }
                }
                return info;
            }
        }
        [System.NonSerialized] private T info;
        #endregion fields & properties

        #region methods
        protected abstract T GetInfo();
        protected BuyableObjectItemData(int id, int startPrice, int discount) : base(id, startPrice, discount) { }
        #endregion methods
    }
}
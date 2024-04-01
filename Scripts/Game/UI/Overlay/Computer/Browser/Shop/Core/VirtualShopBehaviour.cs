using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public abstract class VirtualShopBehaviour<T> : MonoBehaviour where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        public abstract ShopData<T> Data { get; }
        private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            CheckData();
            Subscribe();
        }
        protected virtual void OnDisable()
        {
            UnSubscribe();
        }
        private void CheckData()
        {
            if (Data.Items.Count == 0)
                Data.GenerateNewData();
        }
        private void Subscribe()
        {
            if (Data == null) return;
            if (isSubscribed) return;
            isSubscribed = true;
            OnSubscribe();
        }
        private void UnSubscribe()
        {
            if (Data == null) return;
            if (!isSubscribed) return;
            isSubscribed = false;
            OnUnSubscribe();
        }
        protected virtual void OnSubscribe()
        {
            
        }
        protected virtual void OnUnSubscribe()
        {
            
        }
        #endregion methods
    }
}
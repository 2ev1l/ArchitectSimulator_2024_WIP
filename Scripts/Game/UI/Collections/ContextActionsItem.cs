using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Collections
{
    public abstract class ContextActionsItem<T> : ContextItem<T>
    {
        #region fields & properties
        private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            Subscribe();
        }
        protected virtual void OnDisable()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            if (Context == null) return;
            if (isSubscribed) return;
            isSubscribed = true;
            OnSubscribe();
        }
        private void UnSubscribe()
        {
            if (Context == null) return;
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
        /// <summary>
        /// By default, doesn't invokes on enable
        /// </summary>
        protected virtual void UpdateUI()
        {
            
        }
        public override void OnListUpdate(T param)
        {
            UnSubscribe();
            base.OnListUpdate(param);
            Subscribe();
            UpdateUI();
        }
        #endregion methods
    }
}
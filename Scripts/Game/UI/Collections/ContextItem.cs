using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Game.UI.Collections
{
    public abstract class ContextItem<T> : MonoBehaviour, IListUpdater<T>
    {
        #region fields & properties
        public T Context => context;
        private T context;
        #endregion fields & properties

        #region methods
        public virtual void OnListUpdate(T param)
        {
            this.context = param;
        }
        #endregion methods
    }
}
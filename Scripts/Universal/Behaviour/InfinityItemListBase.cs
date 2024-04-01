using UnityEngine;
using Universal.Collections.Generic;

namespace Universal.Behaviour
{
    public abstract class InfinityItemListBase<Item, UpdateValue> : MonoBehaviour where Item : Component, IListUpdater<UpdateValue>
    {
        #region fields & properties
        public InfinityItemList<Item, UpdateValue> ItemList => itemList;
        [SerializeField] private InfinityItemList<Item, UpdateValue> itemList;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            UpdateListData();
        }
        protected virtual void OnDisable()
        {

        }
        /// <summary>
        /// Invoke <see cref="InfinityItemList.UpdateListDefault()"/> in this method
        /// </summary>
        public abstract void UpdateListData();
        #endregion methods
    }
}
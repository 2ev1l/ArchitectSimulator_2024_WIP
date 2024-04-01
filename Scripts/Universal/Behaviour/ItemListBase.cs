using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Universal.Behaviour
{
    public abstract class ItemListBase<Item, UpdateValue> : MonoBehaviour where Item : Component, IListUpdater<UpdateValue>
    {
        #region fields & properties
        public ItemList<Item, UpdateValue> ItemList => itemList;
        [SerializeField] private ItemList<Item, UpdateValue> itemList;
        public IReadOnlyList<Item> CurrentPageItems => ItemList.CurrentPageItems;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            UpdateListData();
        }
        protected virtual void OnDisable()
        {

        }
        [SerializedMethod]
        public void SwitchPage(bool isNext) => itemList.SwitchPage(isNext);
        /// <summary>
        /// Invoke <see cref="ItemList.UpdateListDefault()"/> in this method
        /// </summary>
        public abstract void UpdateListData();
        #endregion methods
    }
}
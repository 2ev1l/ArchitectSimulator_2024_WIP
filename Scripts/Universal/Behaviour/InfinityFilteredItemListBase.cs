using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Universal.Behaviour
{
    public abstract class InfinityFilteredItemListBase<Item, UpdateValue> : InfinityItemListBase<Item, UpdateValue> where Item : Component, IListUpdater<UpdateValue>
    {
        #region fields & properties
        public IReadOnlyList<UpdateValue> CurrentItems => currentItems;
        protected readonly List<UpdateValue> currentItems = new();
        #endregion fields & properties

        #region methods
        /// <summary>
        /// You should overwrite this items for new values. List can't be null but might be filled with old items data.<br></br>
        /// Use simple <see cref="List{T}.Clear"/> and <see cref="List{T}.Add(T)"/> methods and don't override the class. <br></br>
        /// For default, this method invokes 'OnEnable' from <see cref="UpdateListData"/>
        /// </summary>
        /// <param name="currentItemsReference"></param>
        protected abstract void UpdateCurrentItems(List<UpdateValue> currentItemsReference);
        protected virtual IEnumerable<UpdateValue> GetFilteredItems(IEnumerable<UpdateValue> currentItems)
        {
            return currentItems;
        }
        [SerializedMethod]
        public void UpdateListDataWithFiltersOnly()
        {
            ItemList.UpdateListDefault(GetFilteredItems(CurrentItems), x => x);
        }
        /// <summary>
        /// Updates current items, than filters them
        /// </summary>
        public override void UpdateListData()
        {
            UpdateCurrentItems(currentItems);
            UpdateListDataWithFiltersOnly();
        }
        #endregion methods
    }
}
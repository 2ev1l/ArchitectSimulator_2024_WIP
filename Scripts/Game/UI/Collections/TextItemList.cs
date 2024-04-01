using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Collections
{
    public abstract class TextItemList<I> : ItemListBase<TextItem<I>, I>
    {
        #region fields & properties
        [SerializeField] private List<I> customData = new();
        #endregion fields & properties

        #region methods
        public void SetItems(List<I> items)
        {
            this.customData = items;
            UpdateListData();
        }
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(customData, x => x);
        }
        #endregion methods
    }
}
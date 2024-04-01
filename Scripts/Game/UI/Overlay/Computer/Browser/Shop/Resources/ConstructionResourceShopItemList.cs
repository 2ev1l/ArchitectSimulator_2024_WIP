using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic.Filters;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ConstructionResourceShopItemList : ResourceShopItemList
    {
        #region fields & properties
        [SerializeField] private VirtualFilters<VirtualShopItemContext<ResourceShopItemData>, ConstructionResourceInfo> constructionResourceFilters = new(x => (ConstructionResourceInfo)x.ItemData.Item.Info.ResourceInfo);
        [SerializeField] private DefaultStateMachine constructionTypeStateMachine;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            constructionTypeStateMachine.Context.OnStateChanged += OnStateMachineChanged;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            constructionTypeStateMachine.Context.OnStateChanged -= OnStateMachineChanged;
        }
        protected override IEnumerable<VirtualShopItemContext<ResourceShopItemData>> GetFilteredItems(IEnumerable<VirtualShopItemContext<ResourceShopItemData>> currentItems)
        {
            ConstructionType constructionType = (ConstructionType)constructionTypeStateMachine.Context.CurrentStateId;
            currentItems = currentItems.Where(x => ((ConstructionResourceInfo)x.ItemData.Item.Info.ResourceInfo).ConstructionType == constructionType);
            currentItems = constructionResourceFilters.ApplyFilters(currentItems);
            return base.GetFilteredItems(currentItems);
        }
        private void OnStateMachineChanged(StateChange _)
        {
            UpdateListDataWithFiltersOnly();
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            constructionResourceFilters.Validate(this);
        }
        #endregion methods
    }
}
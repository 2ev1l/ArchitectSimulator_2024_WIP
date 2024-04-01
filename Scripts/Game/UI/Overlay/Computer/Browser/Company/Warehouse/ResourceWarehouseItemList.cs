using Game.DataBase;
using Game.Serialization.World;
using Game.UI.Elements;
using Game.UI.Overlay.Computer.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class ResourceWarehouseItemList : InfinityFilteredItemListBase<ResourceWarehouseItem, ResourceData>
    {
        #region fields & properties
        public WarehouseData Warehouse => GameData.Data.CompanyData.WarehouseData;
        [SerializeField] private VirtualFilters<ResourceData, ResourceInfo> resourceInfoFilters = new(x => x.Info);
        [SerializeField] private CustomButton filtersRefreshButton;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            Warehouse.OnResourceRemoved += UpdateListData;
            Warehouse.OnResourceAdded += UpdateListData;
            filtersRefreshButton.OnClicked += UpdateListDataWithFiltersOnly;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            Warehouse.OnResourceRemoved -= UpdateListData;
            Warehouse.OnResourceAdded -= UpdateListData;
            filtersRefreshButton.OnClicked -= UpdateListDataWithFiltersOnly;
        }
        protected override IEnumerable<ResourceData> GetFilteredItems(IEnumerable<ResourceData> currentItems)
        {
            currentItems = resourceInfoFilters.ApplyFilters(currentItems);
            return base.GetFilteredItems(currentItems);
        }
        private void UpdateListData(ResourceData _) => UpdateListData();
        protected override void UpdateCurrentItems(List<ResourceData> currentItemsReference)
        {
            currentItemsReference.Clear();
            int totalCount = Warehouse.Resources.Count;
            for (int i = 0; i < totalCount; ++i)
            {
                currentItemsReference.Add(Warehouse.Resources[i]);
            }
        }
        public override void UpdateListData()
        {
            base.UpdateListData();
        }
        protected virtual void OnValidate()
        {
            resourceInfoFilters.Validate(this);
        }
        #endregion methods
    }
}
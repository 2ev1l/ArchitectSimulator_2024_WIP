using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class ConstructionResourceBlueprintItemList : InfinityFilteredItemListBase<ConstructionResourceBlueprintItem, ResourceData>
    {
        #region fields & properties
        private WarehouseData WarehouseData => GameData.Data.CompanyData.WarehouseData;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            WarehouseData.OnResourceRemoved += UpdateListData;
            WarehouseData.OnResourceAdded += UpdateListData;
            BlueprintEditor.Instance.OnCurrentDataChanged += UpdateListData;
            BlueprintEditor.Instance.Creator.OnFloorChanged += UpdateListData;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            WarehouseData.OnResourceRemoved -= UpdateListData;
            WarehouseData.OnResourceAdded -= UpdateListData;
            BlueprintEditor.Instance.OnCurrentDataChanged -= UpdateListData;
            BlueprintEditor.Instance.Creator.OnFloorChanged -= UpdateListData;
        }
        protected override IEnumerable<ResourceData> GetFilteredItems(IEnumerable<ResourceData> currentItems)
        {
            BlueprintEditor editor = BlueprintEditor.Instance;
            if (!editor.CanOpenEditor())
            {
                return base.GetFilteredItems(currentItems);
            }
            BuildingFloor currentFloor = editor.Creator.CurrentBuildingFloor;
            BuildingStyle currentStyle = editor.CurrentData.BuildingData.BuildingStyle;
            BuildingType currentType = editor.CurrentData.BuildingData.BuildingType;
            ConstructionType constructionType = ConstructionType.Wall;
            if (currentFloor == BuildingFloor.F1_Flooring || currentFloor == BuildingFloor.F2_FlooringRoof)
            {
                constructionType = ConstructionType.Floor;
            }
            if (editor.CurrentData.BuildingData.MaxFloor == currentFloor)
            {
                constructionType = ConstructionType.Roof;
            }
            currentItems = currentItems.Where(x => FilterItems(x, currentFloor, currentStyle, currentType, constructionType));
            return base.GetFilteredItems(currentItems);
        }
        private bool FilterItems(ResourceData x, BuildingFloor currentFloor, BuildingStyle currentStyle, BuildingType currentType, ConstructionType constructionType)
        {
            ConstructionResourceInfo info = ((ConstructionResourceInfo)x.Info);
            return info.BuildingStyle.HasFlag(currentStyle) &&
                   info.BuildingType.HasFlag(currentType) &&
                   info.BuildingFloor.HasFlag(currentFloor) &&
                   info.ConstructionType == constructionType;
        }
        private void UpdateListData(ResourceData _) => UpdateListData();
        protected override void UpdateCurrentItems(List<ResourceData> currentItemsReference)
        {
            currentItemsReference.Clear();
            if (!BlueprintEditor.Instance.CanOpenEditor())
            {
                return;
            }
            foreach (ResourceData el in WarehouseData.Resources)
            {
                if (el.ResourceType != ResourceType.Construction) continue;
                currentItemsReference.Add(el);
            }
        }
        #endregion methods

    }
}
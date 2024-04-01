using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class BlueprintRoomMarkerItemList : InfinityFilteredItemListBase<BlueprintRoomMarkerItem, BuildingRoom>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            BlueprintEditor.Instance.OnCurrentDataChanged += UpdateListData;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            BlueprintEditor.Instance.OnCurrentDataChanged -= UpdateListData;
        }
        protected override void UpdateCurrentItems(List<BuildingRoom> currentItemsReference)
        {
            currentItemsReference.Clear();
            currentItemsReference.AddRange(BlueprintEditor.Instance.CurrentData.BuildingData.BuildingType.GetAllowedRooms());
        }
        public override void UpdateListData()
        {
            if (!BlueprintEditor.Instance.CanOpenEditor()) return;
            base.UpdateListData();
        }
        #endregion methods
    }
}
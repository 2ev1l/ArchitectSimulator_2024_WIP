using Game.DataBase;
using Game.UI.Collections;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class BlueprintRoomMarkerItem : ContextActionsItem<BuildingRoom>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI roomText;
        [SerializeField] private CustomButton spawnButton;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            spawnButton.OnClicked += SpawnPlaceableRoomItem;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            spawnButton.OnClicked -= SpawnPlaceableRoomItem;
        }
        private void SpawnPlaceableRoomItem()
        {
            BlueprintRoomMarkerPlacer marker = BlueprintEditor.Instance.Creator.CurrentFloor.SpawnRoomMarker(Context, BlueprintEditor.Instance.ViewCenter);
            marker.CheckDeepPlacementSmoothly();
            BlueprintEditor.Instance.Selector.TrySelectElement(marker);
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            roomText.text = Context.ToLanguage();
        }
        #endregion methods
    }
}
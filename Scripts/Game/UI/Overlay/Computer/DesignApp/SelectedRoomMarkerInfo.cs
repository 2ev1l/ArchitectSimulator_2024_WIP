using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal class SelectedRoomMarkerInfo : SelectedBlueprintInfo<BlueprintRoomMarkerPlacer>
    {
        #region fields & properties
        [Title("Room Marker")]
        [SerializeField] private TextMeshProUGUI roomMarkerName;
        [SerializeField] private TextMeshProUGUI roomSizeText;
        [SerializeField] private TextMeshProUGUI roomNameText;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            Context.OnBaseRoomChanged += OnUpdateVisibleUI;
        }
        protected override void OnUnsubscribe()
        {
            base.OnUnsubscribe();
            Context.OnBaseRoomChanged -= OnUpdateVisibleUI;
        }

        protected override void OnUpdateVisibleUI()
        {
            base.OnUpdateVisibleUI();
            roomMarkerName.text = Context.MarkerTypeText;
            if (Context.RoomPlaced == null)
            {
                roomSizeText.text = "0 m2";
                roomNameText.text = "?";
                return;
            }
            roomSizeText.text = Context.RoomPlaced.SizeText;
            roomNameText.text = Context.RoomPlaced.RoomNameText;
        }
        #endregion methods
    }
}
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal class BlueprintRoomMarkerPlacer : BlueprintPlacerBase
    {
        #region fields & properties
        public UnityAction OnBaseRoomChanged;
        public BlueprintRoomMarker Marker
        {
            get
            {
                if (marker == null)
                    TryGetComponent(out marker);
                return marker;
            }
        }
        [SerializeField] private BlueprintRoomMarker marker;
        public string MarkerTypeText => typeText.text;
        [SerializeField] private TextMeshProUGUI typeText;
        public BlueprintRoom RoomPlaced => roomPlaced;
        private BlueprintRoom roomPlaced = null;

        private readonly HashSet<BlueprintRoom> lastCollidedRooms = new();
        private BuildingRoom UIUpdatedRoomType = BuildingRoom.Unknown;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            Marker.OnRoomTypeChanged += UpdateUI;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            Marker.OnRoomTypeChanged += UpdateUI;
        }
        public void RemoveBaseRoom(BlueprintRoom room)
        {
            if (roomPlaced != room) return;
            roomPlaced = null;
            OnBaseRoomChanged?.Invoke();
        }
        public void SetAsBaseRoom(BlueprintRoom room)
        {
            if (roomPlaced == room) return;
            roomPlaced = room;
            OnBaseRoomChanged?.Invoke();
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            if (Marker.RoomType == BuildingRoom.Unknown || Marker.RoomType == UIUpdatedRoomType) return;
            UIUpdatedRoomType = Marker.RoomType;
            typeText.text = $"{Marker.RoomType.ToLanguage()}";
        }
        protected override void CheckFastPlacement(out HashSet<BlueprintPlacerBase> lastCollidedBlueprints, out HashSet<BlueprintRoom> lastCollidedRooms)
        {
            RemoveMarkerFromLastRooms();
            base.CheckFastPlacement(out lastCollidedBlueprints, out lastCollidedRooms);
            if (!base.IsGoodPlacement) return;
            lastCollidedRooms.SetElementsTo(this.lastCollidedRooms);
            RecalculateLastRoomsMarkers();
        }
        private void RemoveMarkerFromLastRooms()
        {
            foreach (BlueprintRoom room in lastCollidedRooms)
            {
                room.TryRemoveMarker(this);
            }
        }
        private void RecalculateLastRoomsMarkers()
        {
            foreach (BlueprintRoom room in lastCollidedRooms)
            {
                room.UpdateMarkers();
            }
        }
        public override void RemoveBlueprint()
        {
            BlueprintEditor.Instance.Creator.CurrentFloor.RemoveBlueprintRoomMarker(this);
            BlueprintEditor.Instance.Creator.CurrentFloor.RoomMarkersPool.TryFindActiveObject(out BlueprintPlacerBase placer);
            BlueprintEditor.Instance.Selector.TrySelectElement(placer);
        }
        public override void CloneBlueprint()
        {
            BlueprintRoomMarkerPlacer placer = BlueprintEditor.Instance.Creator.CurrentFloor.SpawnRoomMarker(Marker.RoomType, Transform.localPosition);
            BlueprintEditor.Instance.Selector.TrySelectElement(placer);
            placer.CheckDeepPlacementSmoothly();
        }
        #endregion methods
    }
}
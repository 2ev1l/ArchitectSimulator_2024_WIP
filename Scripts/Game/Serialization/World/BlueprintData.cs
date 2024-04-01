using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.DataBase;
using UnityEngine.Events;
using System.Linq;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class BlueprintData
    {
        #region fields & properties
        public UnityAction OnNameChanged;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnNameChanged?.Invoke();
            }
        }
        [SerializeField] private string name = "";
        public BuildingData BuildingData => buildingData;
        [SerializeField] private BuildingData buildingData = null;
        public IReadOnlyList<BlueprintResourceData> BlueprintResources => blueprintResources;
        [SerializeField] private List<BlueprintResourceData> blueprintResources = new();
        public IReadOnlyList<BlueprintZoneData> BlueprintZones => blueprintZones;
        [SerializeField] private List<BlueprintZoneData> blueprintZones = new();
        public IReadOnlyList<BlueprintRoomMarkerData> BlueprintRoomMarkers => blueprintRooms;
        [SerializeField] private List<BlueprintRoomMarkerData> blueprintRooms = new();
        #endregion fields & properties

        #region methods
        public bool IsDataClear() => blueprintResources.Count == 0 && blueprintZones.Count == 0 && blueprintRooms.Count == 0;
        /// <summary>
        /// Prepare for <see cref="AddResource(BlueprintResource)"/> and <see cref="AddZone(BlueprintZone)"/>
        /// </summary>
        public void ClearData()
        {
            blueprintResources.Clear();
            blueprintZones.Clear();
            blueprintRooms.Clear();
        }
        /// <summary>
        /// Make sure data is clear before adding
        /// </summary>
        /// <param name="element"></param>
        public void AddResource(BlueprintResource element, BuildingFloor floorPlaced)
        {
            BlueprintResourceData bd = new(element.Transform.localPosition, element.RotationScale, floorPlaced, element.ConstructionReferenceId, element.ChoosedColor);
            blueprintResources.Add(bd);
        }

        /// <summary>
        /// Make sure data is clear before adding
        /// </summary>
        public void AddZone(BlueprintZone zone, BuildingFloor floorPlaced)
        {
            BlueprintZoneData bz = new(zone.Transform.localPosition, zone.LocalTexturePoints.ToList(), floorPlaced, zone.IsGoodPlacement);
            blueprintZones.Add(bz);
        }
        /// <summary>
        /// Make sure data is clear before adding
        /// </summary>
        public void AddRoomMarker(BlueprintRoomMarker roomMarker, BuildingFloor floorPlaced)
        {
            BlueprintRoomMarkerData rd = new(roomMarker.Transform.localPosition, 0, floorPlaced, roomMarker.RoomType);
            blueprintRooms.Add(rd);
        }
        public BlueprintData(string name, BuildingData buildingData)
        {
            this.name = name;
            this.buildingData = buildingData;
        }
        #endregion methods
    }
}
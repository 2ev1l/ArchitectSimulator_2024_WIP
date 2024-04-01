using Game.DataBase;
using Game.Serialization.World;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Collections.Generic;

namespace Game.UI.Overlay.Computer.DesignApp
{
    [System.Serializable]
    internal partial class BlueprintEditorCreator
    {
        #region fields & properties
        public UnityAction OnFloorChanged;
        public UnityAction OnAnyPlacerAdded;
        public UnityAction OnAnyPlacerRemoved;

        [SerializeField] private BlueprintResourcePlacer blueprintPlacerTemplate;
        [SerializeField] private BlueprintZone blueprintZoneTemplate;
        [SerializeField] private BlueprintRoom blueprintRoomTemplate;
        [SerializeField] private BlueprintRoomMarkerPlacer blueprintRoomMarkerTemplate;
        public RectTransform ElementsParent => elementsParent;
        [SerializeField] private RectTransform elementsParent;
        public Floor CurrentFloor
        {
            get
            {
                if (currentFloor == null)
                {
                    Floors.TryGetValue(CurrentBuildingFloor, out currentFloor);
                }
                return currentFloor;
            }
        }
        private Floor currentFloor = null;
        public BuildingFloor CurrentBuildingFloor => currentBuildingFloor;
        private BuildingFloor currentBuildingFloor = BuildingFloor.F1_Flooring;
        public Dictionary<BuildingFloor, Floor> Floors
        {
            get
            {
                floors ??= GetNewFloors();
                return floors;
            }
        }
        private Dictionary<BuildingFloor, Floor> floors = null;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// resourcesIdCount will be cleared and filled with
        /// </summary>
        /// <param name="resourcesIdCount"></param>
        public void GetAllResourcesRequirements(Dictionary<ResourceData, int> resourcesIdCount)
        {
            resourcesIdCount.Clear();
            BuildingFloor maxFloor = BlueprintEditor.Instance.CurrentData.BuildingData.MaxFloor;
            BuildingFloor startFloor = BuildingFloor.F1_Flooring;
            while (maxFloor != startFloor)
            {
                Floors.TryGetValue(startFloor, out Floor floor);
                foreach (ObjectPool<BlueprintPlacerBase> pool in floor.ResourcesPool.Values)
                {
                    int count = 0;
                    foreach (BlueprintPlacerBase placer in pool.Objects)
                    {
                        if (!placer.IsUsing) continue;
                        count++;
                    }
                    if (count == 0) continue;
                    ResourceData resData = ((BlueprintResourcePlacer)pool.OriginalPrefab).ResourceData;
                    if (!resourcesIdCount.TryAdd(resData, count))
                    {
                        resourcesIdCount[resData] += count;
                    }
                }
                startFloor = startFloor.GetNextFloor();
            }
        }
        public void ResetFloor() => ChangeFloor(BuildingFloor.F1_Flooring);
        /// <summary>
        /// Increases floor if max not reached and spawns zones based on current floor
        /// </summary>
        /// <param name="maxFloor"></param>
        public void TryIncreaseFloor(BuildingFloor maxFloor)
        {
            if (maxFloor == CurrentBuildingFloor) return;
            BuildingFloor nextFloor = CurrentBuildingFloor.GetNextFloor();
            if (nextFloor == 0) return;
            Floors.TryGetValue(nextFloor, out Floor next);
            CurrentFloor.GenerateZonesFor(next, out _);
            ChangeFloor(nextFloor);
        }
        public void TryDecreaseFloor()
        {
            BuildingFloor prevFloor = CurrentBuildingFloor.GetPrevFloor();
            if (prevFloor == 0) return;
            ChangeFloor(prevFloor);
        }
        private void ChangeFloor(BuildingFloor newFloor)
        {
            if (newFloor == CurrentBuildingFloor) return;
            CurrentFloor.HideBlueprintObjects();
            currentBuildingFloor = newFloor;
            Floors.TryGetValue(newFloor, out currentFloor);
            CurrentFloor.ShowBlueprintObjects();
            OnFloorChanged?.Invoke();
        }
        /// <summary>
        /// Entire clear all floors with ZONES, ROOMS, RESOURCES and MARKERS
        /// </summary>
        public void ClearAllFloors()
        {
            foreach (Floor floor in Floors.Values)
            {
                floor.RemoveAllBlueprintObjects();
            }
        }
        /// <summary>
        /// Reloads objects from all floors and hides objects (excluding first floor)
        /// </summary>
        /// <param name="currentData"></param>
        public void ReloadAllFloors(BlueprintData currentData)
        {
            ResetFloor();
            foreach (Floor floor in Floors.Values)
            {
                floor.RemoveAllBlueprintObjects();
                floor.DeserializeBlueprintObjects(currentData);
                if (floor.FloorIndex == CurrentBuildingFloor) continue;
                floor.HideBlueprintObjects();
            }
        }
        private Dictionary<BuildingFloor, Floor> GetNewFloors()
        {
            Dictionary<BuildingFloor, Floor> result = new()
            {
                { BuildingFloor.F1_Flooring, new(this, BuildingFloor.F1_Flooring) },
                { BuildingFloor.F1, new(this, BuildingFloor.F1) },
                { BuildingFloor.F2_FlooringRoof, new(this, BuildingFloor.F2_FlooringRoof) },
                { BuildingFloor.F2, new(this, BuildingFloor.F2) },
                { BuildingFloor.F3_Roof, new(this, BuildingFloor.F3_Roof) }
            };
            return result;
        }
        #endregion methods
    }
}
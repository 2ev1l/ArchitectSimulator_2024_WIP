using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class ConstructionData
    {
        #region fields & properties
        public UnityAction OnBuildCompleted;
        public string Name => name;
        [SerializeField] private string name = "";
        public BuildingData BuildingData => buildingData;
        [SerializeField] private BuildingData buildingData = null;
        public IReadOnlyList<BlueprintResourceData> BlueprintResources => blueprintResources;
        [SerializeField] private List<BlueprintResourceData> blueprintResources = new();
        public IReadOnlyList<BlueprintRoomData> BlueprintRooms => blueprintRooms;
        [SerializeField] private List<BlueprintRoomData> blueprintRooms = new();
        public bool IsBuilded => isBuilded;
        [SerializeField] private bool isBuilded = false;
        #endregion fields & properties

        #region methods
        public void CompleteBuild()
        {
            if (isBuilded) return;
            isBuilded = true;
            OnBuildCompleted?.Invoke();
        }
        public ConstructionData(BlueprintData blueprintData, List<BlueprintRoomData> blueprintRooms)
        {
            this.name = blueprintData.Name;
            this.buildingData = blueprintData.BuildingData;
            this.blueprintResources = blueprintData.BlueprintResources.ToList();
            this.blueprintRooms = blueprintRooms.ToList();
        }
        #endregion methods
    }
}
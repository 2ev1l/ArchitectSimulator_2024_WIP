using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class BuildingData
    {
        #region fields & properties
        public BuildingReference BuildingReference => buildingReference;
        [SerializeField] private BuildingReference buildingReference;
        public BuildingType BuildingType => buildingType;
        [SerializeField] private BuildingType buildingType = BuildingType.House;
        public BuildingStyle BuildingStyle => buildingStyle;
        [SerializeField] private BuildingStyle buildingStyle = BuildingStyle.American;
        public BuildingFloor MaxFloor => maxFloor;
        [SerializeField] private BuildingFloor maxFloor;
        public int Floor2Count => floor2Count;
        [SerializeField] private int floor2Count = 0;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Make sure that no one is set as multiple flag
        /// </summary>
        /// <param name="buildingType"></param>
        /// <param name="buildingStyle"></param>
        /// <param name="maxFloor"></param>
        /// <param name="floor2Count"></param>
        public BuildingData(BuildingType buildingType, BuildingStyle buildingStyle, BuildingFloor maxFloor, int floor2Count, BuildingReference buildingReference)
        {
            this.buildingType = buildingType;
            this.buildingStyle = buildingStyle;
            this.maxFloor = maxFloor;
            this.floor2Count = floor2Count;
            this.buildingReference = buildingReference;
        }
        #endregion methods
    }
}
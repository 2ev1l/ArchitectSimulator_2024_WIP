using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public struct BlueprintRoomMarkerData
    {
        #region fields & properties
        public readonly BlueprintUnitData UnitData => unitData;
        [SerializeField] private BlueprintUnitData unitData;
        public readonly BuildingRoom RoomType => roomType;
        [SerializeField] private BuildingRoom roomType;
        #endregion fields & properties

        #region methods
        public BlueprintRoomMarkerData(Vector3 localPosition, int rotation, BuildingFloor floorPlaced, BuildingRoom roomType)
        {
            this.roomType = roomType;
            this.unitData = new(localPosition, rotation, floorPlaced);
        }
        #endregion methods

    }
}
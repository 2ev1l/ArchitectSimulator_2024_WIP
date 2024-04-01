using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public struct BlueprintRoomData
    {
        #region fields & properties
        public readonly float Area => area;
        [SerializeField] private float area;
        public readonly BuildingRoom RoomType1 => roomType1;
        [SerializeField] private BuildingRoom roomType1;
        public readonly BuildingRoom RoomType2 => roomType2;
        [SerializeField] private BuildingRoom roomType2;
        public readonly BuildingFloor FloorPlaced => floorPlaced;
        [SerializeField] private BuildingFloor floorPlaced;
        #endregion fields & properties

        #region methods
        public BlueprintRoomData(BuildingRoom roomType1, BuildingRoom roomType2, float area, BuildingFloor floorPlaced)
        {
            this.roomType1 = roomType1;
            this.roomType2 = roomType2;
            this.area = area;
            this.floorPlaced = floorPlaced;
        }
        #endregion methods
    }
}
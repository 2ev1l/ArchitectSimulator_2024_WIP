using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.DataBase
{
    public class BlueprintRoomMarker : BlueprintGraphic
    {
        #region fields & properties
        public UnityAction OnRoomTypeChanged;
        public BuildingRoom RoomType => roomType;
        [SerializeField] private BuildingRoom roomType = BuildingRoom.Unknown;
        #endregion fields & properties

        #region methods
        public override void Rotate() { }
        public override void RotateTo(int scale) { }
        public void ChangeRoomType(BuildingRoom roomType)
        {
            this.roomType = roomType;
            OnRoomTypeChanged?.Invoke();
        }
        #endregion methods
    }
}
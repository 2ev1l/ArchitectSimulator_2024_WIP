using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public struct BlueprintUnitData
    {
        #region fields & properties
        public readonly Vector3 LocalPosition => localPosition;
        [SerializeField] private Vector3 localPosition;
        public readonly int Rotation => rotation;
        [SerializeField][Range(0, 3)] private int rotation;
        public readonly BuildingFloor FloorPlaced => floorPlaced;
        [SerializeField] private BuildingFloor floorPlaced;
        #endregion fields & properties

        #region methods
        public BlueprintUnitData(Vector3 localPosition, int rotation, BuildingFloor floorPlaced)
        {
            this.localPosition = localPosition;
            this.rotation = rotation;
            this.floorPlaced = floorPlaced;
        }
        #endregion methods
    }
}
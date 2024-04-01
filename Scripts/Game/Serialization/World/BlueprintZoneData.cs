using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class BlueprintZoneData : BlueprintPolygonUnitData
    {
        #region fields & properties
        public bool IsPlacementAllowed => isPlacementAllowed;
        [SerializeField] private bool isPlacementAllowed;
        #endregion fields & properties

        #region methods
        public BlueprintZoneData(Vector3 localPosition, List<Vector2> texturePoints, BuildingFloor floorPlaced, bool isPlacementAllowed) : base(localPosition, texturePoints, floorPlaced)
        {
            this.isPlacementAllowed = isPlacementAllowed;
        }
        #endregion methods
    }
}
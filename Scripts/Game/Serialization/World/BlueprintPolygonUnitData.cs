using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class BlueprintPolygonUnitData
    {
        #region fields & properties
        public Vector3 LocalPosition => localPosition;
        [SerializeField] private Vector3 localPosition;
        public IReadOnlyList<Vector2> TexturePoints => texturePoints;
        [SerializeField] private List<Vector2> texturePoints = new();
        public BuildingFloor FloorPlaced => floorPlaced;
        [SerializeField] private BuildingFloor floorPlaced;
        #endregion fields & properties

        #region methods
        public BlueprintPolygonUnitData(Vector3 localPosition, List<Vector2> texturePoints, BuildingFloor floorPlaced)
        {
            this.localPosition = localPosition;
            this.texturePoints = texturePoints;
            this.floorPlaced = floorPlaced;
        }
        #endregion methods
    }
}
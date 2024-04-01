using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public struct BlueprintResourceData
    {
        #region fields & properties
        public readonly BlueprintUnitData UnitData => unitData;
        [SerializeField] private BlueprintUnitData unitData;
        public readonly int Id => id;
        [SerializeField] private int id;
        public readonly int ChoosedColorId => choosedColorId;
        [SerializeField] private int choosedColorId;
        public readonly ConstructionResourceInfo ResourceInfo => DB.Instance.ConstructionResourceInfo[id].Data;
        #endregion fields & properties

        #region methods
        public BlueprintResourceData(Vector3 localPosition, int rotation, BuildingFloor floorPlaced, int constructionId, int colorId)
        {
            this.id = constructionId;
            this.choosedColorId = colorId;
            this.unitData = new(localPosition, rotation, floorPlaced);
        }
        #endregion methods
    }
}
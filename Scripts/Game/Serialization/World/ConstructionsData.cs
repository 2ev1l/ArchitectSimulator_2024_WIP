using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class ConstructionsData
    {
        #region fields & properties
        [SerializeField] private List<ConstructionData> constructions = new();
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Adds new entry to data. <br></br>
        /// If entry's building reference is not unique, it will not be added
        /// </summary>
        public bool TryAdd(BlueprintData blueprintData, List<BlueprintRoomData> blueprintRooms)
        {
            BuildingReference reference = blueprintData.BuildingData.BuildingReference;
            int count = constructions.Count;
            for (int i = 0; i < count; ++i)
            {
                if (constructions[i].BuildingData.BuildingReference.SameAs(reference)) return false;
            }
            ConstructionData createdData = new(blueprintData, blueprintRooms);
            constructions.Add(createdData);
            return true;
        }
        public bool TryGet(BuildingReference reference, out ConstructionData data)
        {
            int constructionsCount = constructions.Count;
            for (int i = 0; i < constructionsCount; ++i)
            {
                ConstructionData cd = constructions[i];
                if (cd.BuildingData.BuildingReference.SameAs(reference))
                {
                    data = cd;
                    return true;
                }
            }
            data = null;
            return false;
        }
        #endregion methods
    }
}
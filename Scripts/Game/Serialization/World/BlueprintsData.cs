using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class BlueprintsData
    {
        #region fields & properties
        /// <summary>
        /// Unique entries list by name and building reference
        /// </summary>
        public IReadOnlyList<BlueprintData> Blueprints => blueprints;
        [SerializeField] private List<BlueprintData> blueprints = new();
        #endregion fields & properties

        #region methods
        public bool TryCreateNewBlueprint(string name, BuildingData buildingData, out BlueprintData created, out string lockReason)
        {
            created = null;
            int count = blueprints.Count;
            BuildingReference buildingReference = buildingData.BuildingReference;
            lockReason = "";
            for (int i = 0; i < count; ++i)
            {
                BlueprintData blueprintData = blueprints[i];
                if (blueprintData.Name.Equals(name))
                {
                    lockReason = "1"; //todo
                    return false;
                }
                if (buildingReference.SameAs(blueprintData.BuildingData.BuildingReference))
                {
                    lockReason = "2"; //todo
                    return false;
                }
            }
            created = new(name, buildingData);
            blueprints.Add(created);
            return true;
        }
        #endregion methods
    }
}
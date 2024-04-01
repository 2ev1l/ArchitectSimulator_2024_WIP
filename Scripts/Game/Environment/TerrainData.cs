using DebugStuff;
using EditorCustom.Attributes;
using Game.Serialization.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Serialization;

namespace Game.Environment
{
    public class TerrainData : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Terrain terrain;
        private TerrainSettings Context => SettingsData.Data.TerrainSettings;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            SettingsData.Data.OnTerrainChanged += ApplyChanges;
            ApplyChanges(Context);
        }
        private void OnDisable()
        {
            SettingsData.Data.OnTerrainChanged -= ApplyChanges;
        }
        private void ApplyChanges(TerrainSettings newSettings)
        {
            terrain.detailObjectDensity = newSettings.DetailsDensity;
            terrain.treeLODBiasMultiplier = newSettings.TreesQuality;
        }
        #endregion methods
    }
}
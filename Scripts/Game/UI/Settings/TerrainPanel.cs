using Universal.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Collections;

namespace Game.UI.Settings
{
    public class TerrainPanel : SettingsPanel<TerrainSettings>
    {
        #region fields & properties
        protected override TerrainSettings Settings
        {
            get => Context.TerrainSettings;
            set => Context.TerrainSettings = value;
        }
        [SerializeField] private Slider detailsDensitySlider;
        [SerializeField] private Slider treesQualitySlider;

        #endregion fields & properties

        #region methods
        public override TerrainSettings GetNewSettings()
        {
            float dd = ExposeSlider(detailsDensitySlider);
            float tq = ExposeSlider(treesQualitySlider);
            TerrainSettings newSettings = new(dd, tq);
            return newSettings;
        }

        public override void UpdateUI()
        {
            detailsDensitySlider.value = Settings.DetailsDensity;
            treesQualitySlider.value = Settings.TreesQuality;
        }
        #endregion methods
    }
}
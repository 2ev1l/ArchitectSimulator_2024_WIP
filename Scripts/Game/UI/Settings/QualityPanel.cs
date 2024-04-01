using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using QualitySettings = Universal.Serialization.QualitySettings;
using Game.UI.Collections;

namespace Game.UI.Settings
{
    public class QualityPanel : SettingsPanel<QualitySettings>
    {
        #region fields & properties
        protected override QualitySettings Settings
        {
            get => Context.QualitySettings;
            set => Context.QualitySettings = value;
        }

        [SerializeField] private IntItemList qualityPresetList;
        [SerializeField] private IntItemList msaaList;
        [SerializeField] private Slider lightsLimitSlider;
        [SerializeField] private Slider renderScaleSlider;
        [SerializeField] private Slider shadowDistanceSlider;
        [SerializeField] private Slider shadowCascadeSlider;

        [SerializeField] private GameObject raycastBlock;
        [SerializeField] private List<UniversalRenderPipelineAsset> URPAssets;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            qualityPresetList.ItemList.OnPageSwitched += UpdateUIBasedOnPreset;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            qualityPresetList.ItemList.OnPageSwitched -= UpdateUIBasedOnPreset;
        }
        public override QualitySettings GetNewSettings()
        {
            int msaa = ExposeItemList(msaaList);
            float renderScale = ExposeSlider(renderScaleSlider);
            int lightsLimit = (int)ExposeSlider(lightsLimitSlider);
            int shadowDistance = (int)ExposeSlider(shadowDistanceSlider);
            int shadowCascade = (int)ExposeSlider(shadowCascadeSlider);
            QualitySettings newSettings = new(msaa, renderScale, lightsLimit, shadowDistance, shadowCascade, IsCustomAsset());
            return newSettings;
        }

        public override void UpdateUI()
        {
            int urpAssetsCount = URPAssets.Count;
            List<int> qualityPresets = new();
            for (int i = 0; i < urpAssetsCount; ++i)
                qualityPresets.Add(i);

            int currentURP = UnityEngine.QualitySettings.GetQualityLevel();
            qualityPresetList.SetItems(qualityPresets);
            qualityPresetList.ItemList.ShowAt(currentURP);
            UpdateUIBasedOnPreset();
        }
        private void UpdateUIBasedOnPreset()
        {
            int currentURP = ExposeItemList(qualityPresetList);
            int currentMSAA = 0;
            float currentRenderScale = 1;
            int currentLightsLimit = 1;
            int currentShadowDistance = 50;
            int currentShadowCascade = 1;

            if (IsCustomAsset())
            {
                raycastBlock.SetActive(false);
                currentMSAA = Settings.MSAA;
                currentRenderScale = Settings.RenderScale;
                currentLightsLimit = Settings.LightsLimit;
                currentShadowDistance = Settings.ShadowDisance;
                currentShadowCascade = Settings.ShadowCascade;
            }
            else
            {
                raycastBlock.SetActive(true);
                UniversalRenderPipelineAsset urp = URPAssets[currentURP];
                currentMSAA = urp.msaaSampleCount;
                currentRenderScale = urp.renderScale;
                currentLightsLimit = urp.maxAdditionalLightsCount;
                currentShadowDistance = (int)urp.shadowDistance;
                currentShadowCascade = urp.shadowCascadeCount;
            }

            List<int> msaaValues = new() { 1, 2, 4, 8 };
            msaaList.SetItems(msaaValues);
            msaaList.ItemList.ShowAt(currentMSAA);

            lightsLimitSlider.value = currentLightsLimit;
            renderScaleSlider.value = currentRenderScale;
            shadowDistanceSlider.value = currentShadowDistance;
            shadowCascadeSlider.value = currentShadowCascade;
        }
        public override void SetNewSettings()
        {
            UnityEngine.QualitySettings.SetQualityLevel(ExposeItemList(qualityPresetList));
            if (IsCustomAsset())
                base.SetNewSettings();
            else
                Settings.IsCustomAsset = false;
            UpdateUI();
        }
        private bool IsCustomAsset() => ExposeItemList(qualityPresetList) == URPAssets.Count - 1;
        #endregion methods
    }
}
using Game.Serialization.Settings;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Universal.Core;
using Universal.Serialization;
using QualitySettings = Universal.Serialization.QualitySettings;

namespace Game.UI.Overlay
{
    public class ScreenSettings : MonoBehaviour, IStartInitializable, IInitializable
    {
        #region fields & properties
        public static ScreenSettings Instance { get; private set; }
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        public void Start()
        {
            OnGraphicsChanged(SettingsData.Data.GraphicsSettings);
            TryUpdateURPAsset(SettingsData.Data.QualitySettings);
        }
        private void OnEnable()
        {
            SettingsData.Data.OnGraphicsChanged += OnGraphicsChanged;
            SettingsData.Data.OnQualityChanged += TryUpdateURPAsset;
        }
        private void OnDisable()
        {
            SettingsData.Data.OnGraphicsChanged -= OnGraphicsChanged;
            SettingsData.Data.OnQualityChanged -= TryUpdateURPAsset;
        }
        private void OnGraphicsChanged(GraphicsSettings value)
        {
            Screen.SetResolution(value.Resolution.width, value.Resolution.height, value.ScreenMode);
            Application.targetFrameRate = value.RefreshRate;
            UnityEngine.QualitySettings.vSyncCount = value.Vsync ? 1 : 0;
        }
        private void TryUpdateURPAsset(QualitySettings context)
        {
            if (!context.IsCustomAsset) return;
            UniversalRenderPipelineAsset urp = (UniversalRenderPipelineAsset)UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            UpdateURPAsset(context, urp);
        }
        private void UpdateURPAsset(QualitySettings context, UniversalRenderPipelineAsset urp)
        {
            urp.msaaSampleCount = context.MSAA;
            urp.renderScale = context.RenderScale;
            urp.maxAdditionalLightsCount = context.LightsLimit;
            urp.shadowDistance = context.ShadowDisance;
            urp.shadowCascadeCount = context.ShadowCascade;
        }
        #endregion methods
    }
}
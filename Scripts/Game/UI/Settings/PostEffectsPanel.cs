using Universal.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Universal.Serialization.PostEffects;

namespace Game.UI.Settings
{
    public class PostEffectPanel : SettingsPanel<PostEffectSettings>
    {
        #region fields & properties
        protected override PostEffectSettings Settings
        {
            get => Context.PostEffectSettings;
            set => Context.PostEffectSettings = value;
        }
        [SerializeField] private Slider bloomIntensitySlider;
        [SerializeField] private Slider vignetteIntensitySlider;
        [SerializeField] private Slider motionBlurIntensitySlider;
        [SerializeField] private Slider lensFlareIntensitySlider;
        #endregion fields & properties

        #region methods
        public override PostEffectSettings GetNewSettings()
        {
            BloomData b = new(Settings.BloomData.Threshold, ExposeSlider(bloomIntensitySlider));
            VignetteData v = new(ExposeSlider(vignetteIntensitySlider), Settings.VignetteData.Smoothness);
            MotionBlurData m = new(ExposeSlider(motionBlurIntensitySlider));
            LensFlareData l = new(ExposeSlider(lensFlareIntensitySlider));
            PostEffectSettings newSettings = new(b, v, m, Settings.SplitToningData, Settings.WhiteBalanceData, l);
            return newSettings;
        }

        public override void UpdateUI()
        {
            bloomIntensitySlider.value = Settings.BloomData.Intensity;
            vignetteIntensitySlider.value = Settings.VignetteData.Intensity;
            motionBlurIntensitySlider.value = Settings.MotionBlurData.Intensity;
            lensFlareIntensitySlider.value = Settings.LensFlareData.Intensity;
        }
        #endregion methods
    }
}
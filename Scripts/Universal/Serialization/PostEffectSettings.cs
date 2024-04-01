using UnityEngine;
using Universal.Serialization.PostEffects;

namespace Universal.Serialization
{
    [System.Serializable]
    public class PostEffectSettings
    {
        #region fields & properties
        public BloomData BloomData => bloomData;
        [SerializeField] private BloomData bloomData = new();
        public VignetteData VignetteData => vignetteData;
        [SerializeField] private VignetteData vignetteData = new();
        public MotionBlurData MotionBlurData => motionBlurData;
        [SerializeField] private MotionBlurData motionBlurData = new();
        public SplitToningData SplitToningData => splitToningData;
        [SerializeField] private SplitToningData splitToningData = new();
        public WhiteBalanceData WhiteBalanceData => whiteBalanceData;
        [SerializeField] private WhiteBalanceData whiteBalanceData = new();
        public LensFlareData LensFlareData => lensFlareData;
        [SerializeField] private LensFlareData lensFlareData = new();
        #endregion fields & properties

        #region methods
        public PostEffectSettings() { }

        public PostEffectSettings(BloomData bloomData, VignetteData vignetteData, MotionBlurData motionBlurData, SplitToningData splitToningData, WhiteBalanceData whiteBalanceData, LensFlareData lensFlareData)
        {
            this.bloomData = bloomData;
            this.vignetteData = vignetteData;
            this.motionBlurData = motionBlurData;
            this.splitToningData = splitToningData;
            this.whiteBalanceData = whiteBalanceData;
            this.lensFlareData = lensFlareData;
        }
        #endregion methods
    }
}
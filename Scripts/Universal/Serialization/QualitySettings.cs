using UnityEngine;

namespace Universal.Serialization
{
    [System.Serializable]
    public class QualitySettings
    {
        #region fields & properties
        public int MSAA => msaa;
        [SerializeField] private int msaa = 8;
        public float RenderScale => renderScale;
        [SerializeField] private float renderScale = 1;
        public int LightsLimit => lightsLimit;
        [SerializeField] private int lightsLimit = 8;
        public int ShadowDisance => shadowDisance;
        [SerializeField] private int shadowDisance = 200;
        public int ShadowCascade => shadowCascade;
        [SerializeField] private int shadowCascade = 4;
        public bool IsCustomAsset
        {
            get => isCustomAsset;
            set => isCustomAsset = value;
        }
        [SerializeField] private bool isCustomAsset = false;
        #endregion fields & properties

        #region methods
        public QualitySettings() { }

        public QualitySettings(int msaa, float renderScale, int lightsLimit, int shadowDisance, int shadowCascade, bool isCustomAsset)
        {
            this.msaa = msaa;
            this.renderScale = renderScale;
            this.lightsLimit = lightsLimit;
            this.shadowDisance = shadowDisance;
            this.shadowCascade = shadowCascade;
            this.isCustomAsset = isCustomAsset;
        }
        #endregion methods
    }
}
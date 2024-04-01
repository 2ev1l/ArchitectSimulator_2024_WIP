using UnityEngine;

namespace Universal.Serialization.PostEffects
{
    [System.Serializable]
    public class LensFlareData
    {
        #region fields & properties
        /// <summary>
        /// 0..inf
        /// </summary>
        public float Intensity => intensity;
        [Min(0)][SerializeField] private float intensity = 0.1f;
        #endregion fields & properties

        #region methods
        public LensFlareData() { }
        public LensFlareData(float intensity)
        {
            this.intensity = intensity;
        }
        #endregion methods
    }
}
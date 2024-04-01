using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Serialization
{
    [System.Serializable]
    public class TerrainSettings
    {
        #region fields & properties
        /// <summary>
        /// 0..1f
        /// </summary>
        public float DetailsDensity => detailsDensity;
        [SerializeField] private float detailsDensity = 1f;

        /// <summary>
        /// 0.001f..10f
        /// </summary>
        public float TreesQuality => treesQuality;
        [SerializeField] private float treesQuality = 1f;
        #endregion fields & properties

        #region methods
        public TerrainSettings() { }

        public TerrainSettings(float detailsDensity, float treesQuality)
        {
            this.detailsDensity = detailsDensity;
            this.treesQuality = treesQuality;
        }
        #endregion methods
    }
}
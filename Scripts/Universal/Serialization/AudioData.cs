using UnityEngine;
using UnityEngine.Events;

namespace Universal.Serialization
{
    [System.Serializable]
    public class AudioData
    {
        #region fields & properties
        public UnityAction<float> OnVolumeChanged;

        /// <summary>
        /// 0..1f
        /// </summary>
        public float Volume
        {
            get => volume;
            set => SetVolume(value);
        }
        [SerializeField] private float volume = 0.5f;
        #endregion fields & properties

        #region methods
        private void SetVolume(float value)
        {
            if (value < 0 || value > 1)
                throw new System.ArgumentOutOfRangeException("volume");
            volume = value;
            OnVolumeChanged?.Invoke(value);
        }
        #endregion methods
    }
}
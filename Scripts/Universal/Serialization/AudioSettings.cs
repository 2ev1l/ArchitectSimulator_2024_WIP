using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Serialization
{
    [System.Serializable]
    public class AudioSettings
    {
        #region fields & properties
        public AudioData SoundData => soundData;
        [SerializeField] private AudioData soundData = new();
        public AudioData MusicData => musicData;
        [SerializeField] private AudioData musicData = new();
        public AudioData AudioData => audioData;
        [SerializeField] private AudioData audioData = new();
        #endregion fields & properties
    }
}
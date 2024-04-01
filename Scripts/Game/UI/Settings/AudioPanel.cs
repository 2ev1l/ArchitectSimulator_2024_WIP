using Universal.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Serialization;
using System.Collections.Generic;
using AudioSettings = Universal.Serialization.AudioSettings;

namespace Game.UI.Settings
{
    public class AudioPanel : SettingsPanel<AudioSettings>
    {
        #region fields & properties
        protected override AudioSettings Settings
        {
            get => Context.AudioSettings;
            set { }
        }
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider soundSlider;
        #endregion fields & properties

        #region methods
        public override AudioSettings GetNewSettings()
        {
            return Settings;
        }
        public override void SetNewSettings()
        {
            Settings.AudioData.Volume = ExposeSlider(masterSlider);
            Settings.MusicData.Volume = ExposeSlider(musicSlider);
            Settings.SoundData.Volume = ExposeSlider(soundSlider);
        }
        public override void UpdateUI()
        {
            masterSlider.value = Settings.AudioData.Volume;
            musicSlider.value = Settings.MusicData.Volume;
            soundSlider.value = Settings.SoundData.Volume;
        }
        #endregion methods
    }
}
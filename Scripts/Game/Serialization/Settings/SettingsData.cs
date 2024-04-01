using UnityEngine;
using UnityEngine.Events;
using Universal.Serialization;
using Game.Serialization.Settings.Input;
using QualitySettings = Universal.Serialization.QualitySettings;
using AudioSettings = Universal.Serialization.AudioSettings;

namespace Game.Serialization.Settings
{
    [System.Serializable]
    public class SettingsData
    {
        #region fields & properties
        public static readonly string SaveName = "settings";
        public static readonly string SaveExtension = ".json";
        public static SettingsData Data
        {
            get => data;
            set => SetData(value);
        }
        private static SettingsData data;

        public UnityAction<LanguageSettings> OnLanguageChanged;
        public UnityAction<GraphicsSettings> OnGraphicsChanged;
        public UnityAction<PostEffectSettings> OnPostEffectsChanged;
        public UnityAction<QualitySettings> OnQualityChanged;
        public UnityAction<TerrainSettings> OnTerrainChanged;

        [SerializeField] private LanguageSettings languageSettings = new();
        [SerializeField] private GraphicsSettings graphicsSettings = new();
        [SerializeField] private PostEffectSettings postEffectSettings = new();
        [SerializeField] private QualitySettings qualitySettings = new();
        [SerializeField] private TerrainSettings terrainSettings = new();

        [SerializeField] private AudioSettings audioSettings = new();
        [SerializeField] private KeyCodeSettings keyCodeSettings = new();
        [SerializeField] private bool isFirstLaunch = true;

        public LanguageSettings LanguageSettings
        {
            get => languageSettings;
            set => SetLanguage(value);
        }
        public GraphicsSettings GraphicsSettings
        {
            get => graphicsSettings;
            set => SetGraphics(value);
        }
        public AudioSettings AudioSettings
        {
            get => audioSettings;
        }
        public PostEffectSettings PostEffectSettings
        {
            get => postEffectSettings;
            set => SetPostEffects(value);
        }
        public KeyCodeSettings KeyCodeSettings
        {
            get => keyCodeSettings;
        }
        public QualitySettings QualitySettings
        {
            get => qualitySettings;
            set => SetQuality(value);
        }
        public TerrainSettings TerrainSettings
        {
            get => terrainSettings;
            set => SetTerrain(value);
        }
        public bool IsFirstLaunch
        {
            get => isFirstLaunch;
            set => isFirstLaunch = value;
        }
        #endregion fields & properties

        #region methods
        private void SetQuality(QualitySettings value)
        {
            qualitySettings = value;
            OnQualityChanged?.Invoke(value);
        }
        private void SetPostEffects(PostEffectSettings value)
        {
            postEffectSettings = value;
            OnPostEffectsChanged?.Invoke(value);
        }
        private void SetLanguage(LanguageSettings value)
        {
            languageSettings = value;
            OnLanguageChanged?.Invoke(value);
        }
        private void SetGraphics(GraphicsSettings value)
        {
            graphicsSettings = value;
            OnGraphicsChanged?.Invoke(value);
        }
        private void SetTerrain(TerrainSettings value)
        {
            terrainSettings = value;
            OnTerrainChanged?.Invoke(value);
        }
        private static void SetData(SettingsData value) => data = value ?? new SettingsData();
        #endregion methods
    }
}
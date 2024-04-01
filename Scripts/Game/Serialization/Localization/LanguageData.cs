using Game.Serialization.Settings;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Serialization.Localization
{
    [System.Serializable]
    public class LanguageData
    {
        #region fields & properties
        public string[] MenuData => menuData;
        [SerializeField][TextArea(0, 30)] private string[] menuData;
        public string[] GameData => gameData;
        [SerializeField][TextArea(0, 30)] private string[] gameData;
        public string[] TasksData => tasksData;
        [SerializeField][TextArea(0, 30)] private string[] tasksData;
        public string[] ResourcesData => resourcesData;
        [SerializeField][TextArea(0, 30)] private string[] resourcesData;
        public string[] SubtitlesData => subtitlesData;
        [SerializeField][TextArea(0, 50)] private string[] subtitlesData;

        public float AverageCharacterReadingPerSecond => averageCharacterReadingPerMinute / 60;
        [SerializeField] private float averageCharacterReadingPerMinute = 1500;

        public static string LanguagePath => Application.dataPath + "/StreamingAssets/Language";
        #endregion fields & properties

        #region methods
        public float GetAverageTextReadTime(string text) => text.Length / AverageCharacterReadingPerSecond + 2;
        public static LanguageData GetLanguage() => GetLanguage(SettingsData.Data.LanguageSettings.ChoosedLanguage);
        public static LanguageData GetLanguage(string lang)
        {
            string json = File.ReadAllText(Path.Combine(LanguagePath, $"{lang}.json"));
            LanguageData data = JsonUtility.FromJson<LanguageData>(json);
            return data;
        }
        public static List<string> GetLanguageNames()
        {
            var diInfo = new DirectoryInfo(LanguagePath);
            var filesInfo = diInfo.GetFiles("*.json");
            List<string> list = new();
            for (int i = 0; i < filesInfo.Length; i++)
                list.Add(filesInfo[i].Name.Remove(filesInfo[i].Name.Length - 5));
            return list;
        }
        #endregion methods
    }
}
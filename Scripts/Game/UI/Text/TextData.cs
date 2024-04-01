using Game.Serialization.Localization;
using Game.Serialization.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Universal.Core;
using Universal.Serialization;

namespace Game.UI.Text
{
    public class TextData : MonoBehaviour, IInitializable, IStartInitializable
    {
        #region fields & properties
        public static TextData Instance { get; private set; }
        [SerializeField] private LanguageData languageData;
        public LanguageData LoadedData
        {
            get
            {
                loadedData ??= languageData;
                return loadedData;
            }
            set => loadedData = value;
        }
        private LanguageData loadedData;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// If you're using this for the first time (without any language save) you must disable this method
        /// </summary>
        public void Init()
        {
            Instance = this;
            LoadChoosedLanguage();
        }
        /// <summary>
        /// If you're using this for the first time (without any language save) you must disable this method
        /// </summary>
        public void Start()
        {
            StartCoroutine(WaitForUpdateText());
        }
        private IEnumerator WaitForUpdateText()
        {
            yield return CustomMath.WaitAFrame();
            UpdateText();
        }
        private void OnEnable()
        {
            SettingsData.Data.OnLanguageChanged += UpdateText;
        }
        private void OnDisable()
        {
            SettingsData.Data.OnLanguageChanged -= UpdateText;
        }
        private void UpdateText(LanguageSettings languageSettings) => UpdateText();
        [ContextMenu("Update Text")]
        private void UpdateText()
        {
            LoadChoosedLanguage();
            FindObjectsByType<LanguageLoader>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ForEach(x => x.Load());
            FindObjectsByType<TextUpdater>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ForEach(x => x.UpdateText());
        }
        private void LoadChoosedLanguage()
        {
            try
            {
                LoadedData = LanguageData.GetLanguage();
            }
            catch
            {
                Debug.LogError("Error - Can't find a language. Settting English by default.");
                SettingsData.Data.LanguageSettings.ResetLanguage();
            }
        }
        public LanguageData GetEnglishData() => languageData;

        [ContextMenu("Save ENGLISH Language")]
        private void SaveLanguage()
        {
            LanguageData data = languageData;
            string json = JsonUtility.ToJson(data);
            string path = Path.Combine(LanguageData.LanguagePath, $"English.json");
            File.WriteAllText(path, json);
            Debug.Log(path + " saved");
        }

        [ContextMenu("Export language to txt")]
        private void ExportToTxt()
        {
            LanguageData data = LoadedData;
            string path = Application.persistentDataPath + "/export.txt";
            string text = "";

            File.WriteAllText(path, text);
        }
        private string GetTextFromArray(string[] array)
        {
            string result = "";
            foreach (var el in array)
            {
                result += $"{LanguageLoader.RemoveXMLTags(el)}\n";
            }

            return result;
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            CheckMultipleArrayEntries(languageData.MenuData, "Menu");
            CheckMultipleArrayEntries(languageData.GameData, "Game");
            CheckMultipleArrayEntries(languageData.TasksData, "Tasks");
            CheckMultipleArrayEntries(languageData.ResourcesData, "Resources");
        }
        private void CheckMultipleArrayEntries(string[] array, string name)
        {
            HashSet<string> equals = array.FindEquals((x, y) => x.Equals(y));
            foreach (var el in equals)
            {
                if (el.Equals("")) continue;
                Debug.LogError($"Multiple text '{el}' in {name}");
            }
        }

#endif //UNITY_EDITOR
    }
}
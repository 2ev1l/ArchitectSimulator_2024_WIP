using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Universal.Serialization
{
    public abstract class SavingUtils : MonoBehaviour, IInitializable, IStartInitializable
    {
        #region fields & properties
        public static SavingUtils Instance { get; private set; }
        public static UnityAction OnBeforeSave;
        public static UnityAction OnAfterSave;
        public static UnityAction OnDataReset;
        public static UnityAction OnSettingsReset;
        public static string StreamingAssetsPath => Application.dataPath + "/StreamingAssets";
        #endregion fields & properties

        #region methods
        public virtual void Init()
        {
            Instance = this;

            CheckSaves();
            LoadAll();
        }
        protected abstract void CheckSaves();
        public static bool IsFileExists(string dataPath, string fullFileName)
        {
            return File.Exists(Path.Combine(dataPath, fullFileName));
        }
        public void Start()
        {
            SaveAll();
        }
        protected void SaveAll()
        {
            if (CanSave())
                SaveGameData();
            SaveSettings();
        }
        private void LoadAll()
        {
            LoadGameData();
            LoadSettings();
        }
        public virtual bool CanSave()
        {
            return true;
        }
        public abstract void SaveGameData();
        public abstract void SaveSettings();

        protected abstract void LoadGameData();
        protected abstract void LoadSettings();

        public abstract void ResetTotalProgress(bool doAction = true);

        public static void SaveJson<T>(T data, string saveName)
        {
            string json = JsonUtility.ToJson(data);
            string path = Path.Combine(Application.persistentDataPath, saveName);
            File.WriteAllText(path, json);
        }
        public static T LoadJson<T>(string saveName)
        {
            string path = Path.Combine(Application.persistentDataPath, saveName);
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        private void OnApplicationQuit()
        {
            SaveAll();
        }
        #endregion methods
    }
}
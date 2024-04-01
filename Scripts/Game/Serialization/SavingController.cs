using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using EditorCustom.Attributes;
using DebugStuff;
using Game.Serialization.World;
using Game.Serialization.Settings;
using Universal;
using Universal.Serialization;

namespace Game.Serialization
{
    public class SavingController : SavingUtils
    {
        #region fields & properties
        #endregion fields & properties

        #region methods
        public override void Init()
        {
            base.Init();
        }
        protected override void CheckSaves()
        {
            if (!IsFileExists(Application.persistentDataPath, GameData.SaveName + GameData.SaveExtension)) ResetTotalProgress();
            if (!IsFileExists(Application.persistentDataPath, SettingsData.SaveName + SettingsData.SaveExtension)) ResetSettings();
        }

        /// <summary>
        /// Probably you also might want to use <see cref="CanSave"/> to avoid save bugs.
        /// </summary>
        public override void SaveGameData()
        {
            OnBeforeSave?.Invoke();
            string rawJson = JsonUtility.ToJson(GameData.Data);
            string json = Cryptography.Encrypt(rawJson);
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, GameData.SaveName + GameData.SaveExtension), FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, json);
                fs.Close();
            }
            OnAfterSave?.Invoke();
        }
        public override void SaveSettings()
        {
            SaveJson(SettingsData.Data, SettingsData.SaveName + SettingsData.SaveExtension);
        }
        protected override void LoadGameData()
        {
            string json;
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, GameData.SaveName + ".data"), FileMode.Open))
            {
                var bf = new BinaryFormatter();
                json = bf.Deserialize(fs).ToString();
                json = Cryptography.Decrypt(json);
                fs.Close();
            }
            GameData.SetData(JsonUtility.FromJson<GameData>(json));
        }
        protected override void LoadSettings()
        {
            SettingsData.Data = LoadJson<SettingsData>(SettingsData.SaveName + SettingsData.SaveExtension);
        }
        private void ResetSettings()
        {
            SettingsData.Data = new SettingsData();
            SaveSettings();
            OnSettingsReset?.Invoke();
            Debug.Log("Settings reset");
        }
        public override void ResetTotalProgress(bool doAction = true)
        {
            GameData.SetData(new GameData());
            Instance.SaveGameData();
            if (doAction)
                OnDataReset?.Invoke();
            Debug.Log("Progress reset");
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [Button(nameof(TestSaveGameData))]
        private void TestSaveGameData()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            SaveGameData();
        }
#endif //UNITY_EDITOR
    }
}
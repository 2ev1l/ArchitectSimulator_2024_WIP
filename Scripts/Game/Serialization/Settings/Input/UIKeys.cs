using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.Settings.Input
{
    [System.Serializable]
    public class UIKeys : KeysCollection
    {
        #region fields & properties
        public KeyCodeInfo SettingsKey => settingsKey;
        [SerializeField] private KeyCodeInfo settingsKey = new(KeyCode.Escape, KeyCodeDescription.OpenSettings);
        public KeyCodeInfo InfoKey => infoKey;
        [SerializeField] private KeyCodeInfo infoKey = new(KeyCode.H, KeyCodeDescription.Info);
        public KeyCodeInfo HideUIKey => hideUIKey;
        [SerializeField] private KeyCodeInfo hideUIKey = new(KeyCode.F1, KeyCodeDescription.HideUI);
        #endregion fields & properties

        #region methods
        public override List<KeyCodeInfo> GetKeys()
        {
            List<KeyCodeInfo> list = new()
            {
                SettingsKey,
                InfoKey,
                HideUIKey
            };
            return list;
        }
        #endregion methods
    }
}
using Game.Serialization.Settings;
using Game.Serialization.Settings.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Universal.Core;
using Universal.Events;

namespace Game.Events
{
    public class InputController : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public static InputController Instance { get; private set; }
        private static KeyCodeSettings Context => SettingsData.Data.KeyCodeSettings;
        public static UnityAction<KeyCodeInfo> OnKeyDown;
        public static UnityAction<KeyCodeInfo> OnKeyUp;
        public static UnityAction<KeyCodeInfo> OnKeyHold;
        public static HashSet<KeyCodeInfo> AllKeys
        {
            get
            {
                allKeys ??= GetKeys(Context);
                return allKeys;
            }
        }
        private static HashSet<KeyCodeInfo> allKeys = null;
        private HashSet<KeyCodeInfo> UIKeys
        {
            get
            {
                uiKeys ??= GetKeys(Context.UIKeys);
                return uiKeys;
            }
        }
        private HashSet<KeyCodeInfo> uiKeys = null;
        private HashSet<KeyCodeInfo> PlayerKeys
        {
            get
            {
                playerKeys ??= GetKeys(Context.PlayerKeys);
                return playerKeys;
            }
        }
        private HashSet<KeyCodeInfo> playerKeys = null;
        private HashSet<KeyCodeInfo> OverlayKeys
        {
            get
            {
                overlayKeys ??= GetKeys(Context.OverlayKeys);
                return overlayKeys;
            }
        }
        private HashSet<KeyCodeInfo> overlayKeys = null;
        private KeyCodeInfo SettingsStaticKey
        {
            get
            {
                if (settingsStaticKey == null)
                    UIKeys.Exists(x => x.Description == KeyCodeDescription.OpenSettings, out settingsStaticKey);
                return settingsStaticKey;
            }
        }
        private KeyCodeInfo settingsStaticKey = null;
        private static GameObject EventSystem
        {
            get
            {
                if (eventSystem == null)
                {
                    eventSystem = GameObject.FindAnyObjectByType<EventSystem>().gameObject;
                }
                return eventSystem;
            }
        }
        private static GameObject eventSystem;
        [SerializeField] private ActionRequest inputPlayer = new();
        [SerializeField] private ActionRequest inputUI = new();
        [SerializeField] private ActionRequest inputOverlay = new();
        [SerializeField] private ActionRequest inputEventSystem = new();
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }

        private static HashSet<KeyCodeInfo> GetKeys(KeysCollection referenceCollection)
        {
            return referenceCollection.GetKeys().ToHashSet();
        }
        public void LockInputSystem(int blockLevel)
        {
            inputEventSystem.AddBlockLevel(blockLevel);
            if (!EventSystem.activeSelf) return;
            EventSystem.SetActive(false);
        }
        public void UnlockInputSystem(int blockLevel)
        {
            inputEventSystem.RemoveBlockLevel(blockLevel);
            bool canExecute = inputEventSystem.CanExecute(int.MinValue);
            if (EventSystem.activeSelf == canExecute) return;
            EventSystem.SetActive(canExecute);
        }
        public void LockFullInput(int blockLevel)
        {
            inputUI.AddBlockLevel(blockLevel);
            inputPlayer.AddBlockLevel(blockLevel);
            inputOverlay.AddBlockLevel(blockLevel);
        }
        public void UnlockFullInput(int blockLevel)
        {
            inputUI.RemoveBlockLevel(blockLevel);
            inputPlayer.RemoveBlockLevel(blockLevel);
            inputOverlay.RemoveBlockLevel(blockLevel);
        }

        public void LockPlayerInput(int blockLevel)
        {
            inputPlayer.AddBlockLevel(blockLevel);
        }
        public void UnlockPlayerInput(int blockLevel)
        {
            inputPlayer.RemoveBlockLevel(blockLevel);
        }

        public void LockUIInput(int blockLevel)
        {
            inputUI.AddBlockLevel(blockLevel);
        }
        public void UnlockUIInput(int blockLevel)
        {
            inputUI.RemoveBlockLevel(blockLevel);
        }

        public void LockOverlayInput(int blockLevel)
        {
            inputOverlay.AddBlockLevel(blockLevel);
        }
        public void UnlockOverlayInput(int blockLevel)
        {
            inputOverlay.RemoveBlockLevel(blockLevel);
        }
        private void Update()
        {
            CheckInputUI();
            CheckInputPlayer();
            CheckOverlayInput();
        }
        private void CheckInputUI()
        {
            if (!inputUI.CanExecute(0))
            {
                if (!inputUI.CanExecute(int.MaxValue)) return;
                CheckKeyActions(SettingsStaticKey);
                return;
            }
            CheckKeysActions(UIKeys);
        }
        private void CheckOverlayInput()
        {
            if (!inputOverlay.CanExecute(0)) return;
            CheckKeysActions(OverlayKeys);
        }
        private void CheckInputPlayer()
        {
            if (!inputPlayer.CanExecute(0)) return;
            CheckKeysActions(PlayerKeys);
        }
        private void CheckKeysActions(HashSet<KeyCodeInfo> keys)
        {
            foreach (KeyCodeInfo key in keys)
                CheckKeyActions(key);
        }
        private void CheckKeyActions(KeyCodeInfo key)
        {
            if (Input.GetKeyDown(key.Key))
                OnKeyDown?.Invoke(key);

            if (Input.GetKeyUp(key.Key))
                OnKeyUp?.Invoke(key);

            if (Input.GetKey(key.Key))
                OnKeyHold?.Invoke(key);
        }
        #endregion methods
    }
}
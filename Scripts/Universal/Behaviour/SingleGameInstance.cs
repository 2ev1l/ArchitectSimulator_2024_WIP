using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Universal.Core;
using Universal.Serialization;
using DebugStuff;

namespace Universal.Behaviour
{
    [ExecuteAlways]
    public class SingleGameInstance : MonoBehaviour
    {
        #region fields
        public static SingleGameInstance Instance => instance;
        private static SingleGameInstance instance;
        private static bool isInitialized = false;
        private bool isMain = false;

        [SerializeField] private SavingUtils savingUtils;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private List<Component> iInitializable;
        [SerializeField] private List<Component> iStartInitializable;
        #endregion fields

        #region methods
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif //UNITY_EDITOR

            SavingUtils.OnDataReset += InitAfterReset;
            SceneLoader.OnSceneLoaded += Awake;
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif //UNITY_EDITOR

            SavingUtils.OnDataReset -= InitAfterReset;
            SceneLoader.OnSceneLoaded -= Awake;
        }
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (automaticallyUpdateEditor)
                    CheckInterfaces();
                return;
            }
#endif //UNITY_EDITOR

            if (!isInitialized)
            {
                isMain = true;
                isInitialized = true;
                instance = this;
                OnInitialize();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (!isMain)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
                OnLoad();
            }
        }
        private void OnInitialize()
        {
            savingUtils.GetComponent<IInitializable>().Init();
            InitAfterReset();
        }

        private void OnLoad()
        {
            sceneLoader.Start();
            savingUtils.GetComponent<IStartInitializable>().Start();
            foreach (var el in iStartInitializable.Cast<IStartInitializable>()) el.Start();
        }
        public void InitAfterReset()
        {
            //init before
            sceneLoader.Init();
            int counter = 0;
            foreach (var el in iInitializable.Cast<IInitializable>())
            {
                el.Init();
                counter++;
            }

            //change state next
            counter = 0;
            foreach (var el in iInitializable.Cast<IInitializable>())
            {
                ChangeObjectState(iInitializable[counter]);
                counter++;
            }
            ChangeObjectState(sceneLoader);
        }
        private void ChangeObjectState(Component component)
        {
            GameObject Object = component.gameObject;
            if (!Object.activeSelf) return;
            Object.SetActive(false);
            Object.SetActive(true);
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!automaticallyUpdateEditor) return;
            CheckInterfaces();
        }
        [SerializeField] private bool automaticallyUpdateEditor = true;
        [ContextMenu("Check Interfaces")]
        private void CheckInterfaces()
        {
            DebugCommands.CastInterfacesList<IStartInitializable>(iStartInitializable, "iStartInitializable", this);
            DebugCommands.CastInterfacesList<IInitializable>(iInitializable, "iInitializable", this);
        }
#endif //UNITY_EDITOR
    }
}
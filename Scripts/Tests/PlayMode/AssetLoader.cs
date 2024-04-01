using Game.UI.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests.PlayMode
{
    [System.Serializable]
    public class AssetLoader
    {
        #region fields & properties
        private static readonly GameObject ddol = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Core/DDOL.prefab");
        public static GameObject SingleGameInstance = null;
        public static GameObject Camera = null;
        #endregion fields & properties

        #region methods
        public static void InitCamera()
        {
            if (!TryInstantiateSingle(new(CanvasInitializer.OverlayCameraName), ref Camera)) return;
            Camera.AddComponent<Camera>();
        }
        public static void InitSingleGameInstance()
        {
            InitCamera();
            TryInstantiateSingle(ddol, ref SingleGameInstance);
        }
        private static bool TryInstantiateSingle(GameObject prefab, ref GameObject reference)
        {
            if (IsInstantiated(reference)) return false;
            reference = Instantiate(prefab);
            return true;
        }
        private static bool IsInstantiated(GameObject obj) => obj != null;
        private static GameObject Instantiate(GameObject obj) => Object.Instantiate(obj);
        #endregion methods
    }
}
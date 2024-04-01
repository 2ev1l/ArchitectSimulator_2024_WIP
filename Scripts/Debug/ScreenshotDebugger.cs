using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorCustom.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DebugStuff
{
    internal class ScreenshotDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        #region fields & properties
        [SerializeField] private GameObject firstCamera;
        [SerializeField] private GameObject secondCamera;
        [SerializeField] private Transform objectsParent;
        private bool isCameraSwitched = false;
        #endregion fields & properties

        #region methods
        [Button(nameof(SwitchCamera))]
        private void SwitchCamera()
        {
            bool camSwitch = !firstCamera.activeSelf;
            firstCamera.SetActive(camSwitch);
            secondCamera.SetActive(!camSwitch);
        }
        [Button(nameof(SetNextChild))]
        private void SetNextChild()
        {
            int childs = objectsParent.childCount;
            bool activateNextChild = false;
            for (int i = 0; i < childs; ++i)
            {
                Transform child = objectsParent.GetChild(i);
                if (activateNextChild)
                {
                    child.gameObject.SetActive(true);
                    break;
                }
                if (child.gameObject.activeSelf)
                {
                    activateNextChild = true;
                    child.gameObject.SetActive(false);
                }
            }
            if (!activateNextChild)
                objectsParent.GetChild(0).gameObject.SetActive(true);
        }
        [Button(nameof(GetNextScreenshot))]
        private void GetNextScreenshot()
        {
            if (isCameraSwitched)
            {
                SetNextChild();
            }
            SwitchCamera();
            isCameraSwitched = !isCameraSwitched;
        }
        #endregion methods
#endif //UNITY_EDITOR
    }
}
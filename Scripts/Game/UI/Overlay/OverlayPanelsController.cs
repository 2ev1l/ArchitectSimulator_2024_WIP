using EditorCustom.Attributes;
using Game.UI.Elements;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;
using Universal.Events;
using System.Linq;
using Game.Events;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Game.UI.Overlay
{
    public class OverlayPanelsController : MonoBehaviour, IInitializable, IRequestExecutor
    {
        #region fields & properties
        public UnityAction OnPanelOpened;
        public UnityAction OnPanelClosed;
        public static OverlayPanelsController Instance { get; private set; }
        [SerializeField] private GameObject raycastBlock;
        [SerializeField] private List<OverlayPanel> overlayPanels;
        public bool IsPanelOpened => isPanelOpened;
        [SerializeField][ReadOnly] private bool isPanelOpened = false;
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            RequestController.Instance.EnableExecution(this);
            overlayPanels.ForEach(x => x.OnPanelClosed += EnableInput);
        }
        private void OnDisable()
        {
            RequestController.Instance.DisableExecution(this);
            overlayPanels.ForEach(x => x.OnPanelClosed -= EnableInput);
        }
        private void EnableInput()
        {
            raycastBlock.SetActive(false);
            InputController.Instance.UnlockFullInput(int.MaxValue);
            isPanelOpened = false;
            OnPanelClosed?.Invoke();
        }
        private void DisableInput()
        {
            raycastBlock.SetActive(true);
            InputController.Instance.LockFullInput(int.MaxValue);
            isPanelOpened = true;
            OnPanelOpened?.Invoke();
        }
        public bool TryExecuteRequest(ExecutableRequest request)
        {
            if (IsPanelOpened) return false;
            bool isExecuted = false;
            foreach (var overlayPanel in overlayPanels)
            {
                if (overlayPanel.TryExecuteRequest(request))
                    isExecuted = true;
            }
            if (isExecuted)
                DisableInput();
            return isExecuted;
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField][DontDraw] private bool ___debugBool;
        [Button(nameof(GetAllPanelsInChild))]
        private void GetAllPanelsInChild()
        {
            Undo.RecordObject(gameObject, "Get All Panels in Child");
            overlayPanels = transform.GetComponentsInChildren<OverlayPanel>(true).ToList();
        }
#endif //UNITY_EDITOR
    }
}
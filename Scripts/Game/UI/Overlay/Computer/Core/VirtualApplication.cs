using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Game.UI.Overlay.Computer.VirtualApplication;

namespace Game.UI.Overlay.Computer
{
    public class VirtualApplication : VirtualStateMachine<ApplicationState>
    {
        #region fields & properties
        public GameObject GameObject
        {
            get
            {
                if (_gameObject == null)
                {
                    _gameObject = gameObject;
                }
                return _gameObject;
            }
        }
        private GameObject _gameObject = null;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = transform;
                }
                return _transform;
            }
        }
        private Transform _transform = null;
        public Sprite Icon => icon;
        [SerializeField] private Sprite icon;
        #endregion fields & properties

        #region methods
        protected bool IsMainVisibleApplication()
        {
            Transform parent = Transform.parent;
            int parentChilds = parent.childCount;
            for (int i = parentChilds - 1; i > -1; --i)
            {
                Transform child = parent.GetChild(i);
                GameObject childGO = child.gameObject;
                if (!childGO.activeSelf) continue;

                if (child == Transform) return true;
                return false;
            }
            return false;
        }
        [SerializedMethod]
        public void StartApplication() => TryApplyState(ApplicationState.Visible);
        [SerializedMethod]
        public void CloseApplication() => TryApplyState(ApplicationState.Closed);
        [SerializedMethod]
        public void HideApplication() => TryApplyState(ApplicationState.Hidden);
        [SerializedMethod]
        public void HideOrShowApplication()
        {
            switch (CurrentState)
            {
                case ApplicationState.Visible: HideApplication(); break;
                case ApplicationState.Hidden: StartApplication(); break;
                default: break;
            }
        }
        protected override void OnStateChanged(ApplicationState newState)
        {
            switch (newState)
            {
                case ApplicationState.Visible:
                    OnVisibleApplication();
                    if (LastState == ApplicationState.Closed)
                        OnFirstStartApplication();
                    break;
                case ApplicationState.Closed: OnCloseApplication(); break;
                case ApplicationState.Hidden: OnHideApplication(); break;
                default: throw new System.NotImplementedException(newState.ToString());
            }
        }
        /// <summary>
        /// Invokes if there's multiple applications opened, and one of them changes state
        /// </summary>
        public virtual void OnViewFocusChanged() { }
        protected virtual void OnFirstStartApplication() { }
        protected virtual void OnVisibleApplication()
        {
            Transform.SetAsLastSibling();
            if (!GameObject.activeSelf)
                GameObject.SetActive(true);
        }
        protected virtual void OnInvisibleApplication()
        {
            if (GameObject.activeSelf)
                GameObject.SetActive(false);
        }
        protected virtual void OnCloseApplication()
        {
            OnInvisibleApplication();
        }
        protected virtual void OnHideApplication()
        {
            OnInvisibleApplication();
        }
        #endregion methods
        public enum ApplicationState
        {
            Closed,
            Visible,
            Hidden
        }
    }
}
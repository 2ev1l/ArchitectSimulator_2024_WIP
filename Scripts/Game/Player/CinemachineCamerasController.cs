using Cinemachine;
using EditorCustom.Attributes;
using Game.Serialization.Settings;
using Game.UI.Overlay;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.Player
{
    public class CinemachineCamerasController : SingleSceneInstance<CinemachineCamerasController>
    {
        #region fields & properties
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private List<CameraInfo> cameras;

        [Title("UI")]
        [SerializeField] private List<StateChange> cameraUnlockedStates;
        private static UIStateMachine UIStateMachine => UIStateMachine.Instance;
        public Transform FollowTarget { get; set; }
        public Transform LookAt { get; set; }

        [Title("Read Only")]
        [SerializeField][ReadOnly] private CameraType lastActiveCamera = CameraType.FPV;
        [SerializeField][ReadOnly] private CameraType activeCamera = CameraType.FPV;
        private static readonly float baseSensitivity = 300;
        private readonly List<CameraType> cursorVisibleCameras = new() { CameraType.Disabled };
        public Transform MainCameraTransform
        {
            get
            {
                if (mainCameraTransform == null)
                {
                    mainCameraTransform = MainCamera.transform;
                }
                return mainCameraTransform;
            }
        }
        private Transform mainCameraTransform = null;
        public Camera MainCamera
        {
            get
            {
                if (mainCamera == null)
                {
                    mainCamera = Camera.main;
                }
                return mainCamera;
            }
        }
        private Camera mainCamera = null;
        #endregion fields & properties

        #region methods
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
                ChangeCursor();
            else
                Cursor.lockState = CursorLockMode.None;
        }
        private void OnEnable()
        {
            ChangeCursor();
            ChangeCameraSettings();
            SettingsData.Data.OnGraphicsChanged += ChangeCameraSettings;
            UIStateMachine.Context.OnStateChanged += CheckUIState;
            OverlayPanelsController.Instance.OnPanelOpened += ChangeCursor;
            OverlayPanelsController.Instance.OnPanelClosed += ChangeCursor;
        }

        private void OnDisable()
        {
            SettingsData.Data.OnGraphicsChanged -= ChangeCameraSettings;
            UIStateMachine.Context.OnStateChanged -= CheckUIState;
            OverlayPanelsController.Instance.OnPanelOpened -= ChangeCursor;
            OverlayPanelsController.Instance.OnPanelClosed -= ChangeCursor;
        }
        private void DisableCameraMove()
        {
            foreach (var cam in cameras)
            {
                CinemachinePOV pov = cam.Camera.GetCinemachineComponent<CinemachinePOV>();
                if (pov == null) continue;
                pov.m_VerticalAxis.m_MaxSpeed = 0;
                pov.m_HorizontalAxis.m_MaxSpeed = 0;
            }
        }
        private void EnableCameraMove()
        {
            ChangeCameraSettings();
        }
        private void CheckUIState(StateChange currentState)
        {
            bool isCameraLocked = !cameraUnlockedStates.Contains(currentState);
            if (isCameraLocked)
            {
                ChangeCamera(CameraType.Disabled);
                return;
            }
            if (lastActiveCamera == CameraType.Disabled)
            {
                ChangeCamera(CameraType.FPV);
                return;
            }
            ChangeCamera(CameraType.FPV);
        }
        private void ChangeCameraSettings(Universal.Serialization.GraphicsSettings newSettings) => ChangeCameraSettings();
        private void ChangeCameraSettings()
        {
            Vector2 sensitivity = SettingsData.Data.GraphicsSettings.CameraSensitvity * baseSensitivity;
            int fov = SettingsData.Data.GraphicsSettings.CameraFOV;
            MainCamera.fieldOfView = fov;
            foreach (var cam in cameras)
            {
                cam.Camera.m_Lens.FieldOfView = fov;
                CinemachinePOV pov = cam.Camera.GetCinemachineComponent<CinemachinePOV>();
                if (pov == null) continue;
                pov.m_VerticalAxis.m_MaxSpeed = sensitivity.y;
                pov.m_HorizontalAxis.m_MaxSpeed = sensitivity.x;
            }
        }
        public void ChangeCamera(CameraType cameraType)
        {
            if (cameraType.Equals(activeCamera)) return;
            foreach (var cam in cameras)
            {
                bool cameraEnabled = cam.Type.Equals(cameraType);
                cam.Camera.gameObject.SetActive(cameraEnabled);
                if (cameraEnabled)
                    ApplyCurrentCameraProperties(cam);
            }
            lastActiveCamera = activeCamera;
            activeCamera = cameraType;
            ChangeCursor();
        }
        private void ApplyCurrentCameraProperties(CameraInfo info)
        {
            switch (info.Type)
            {
                case CameraType.InstantTargetLook:
                    info.Camera.Follow = FollowTarget;
                    info.Camera.LookAt = LookAt;
                    break;
                default: break;
            }
        }
        private void ChangeCursor()
        {
            bool isCameraDrawsCursor = cursorVisibleCameras.Contains(activeCamera);
            bool isUIDrawsCursor = UIStateMachine.Context.CurrentState != UIStateMachine.Context.DefaultState || OverlayPanelsController.Instance.IsPanelOpened;
            Cursor.visible = isCameraDrawsCursor || isUIDrawsCursor;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
            if (OverlayPanelsController.Instance.IsPanelOpened)
                DisableCameraMove();
            else
                EnableCameraMove();
        }
        #endregion methods
        public enum CameraType
        {
            Disabled,
            FPV,
            InstantTargetLook,
        }
        [System.Serializable]
        private class CameraInfo
        {
            public CinemachineVirtualCamera Camera => camera;
            [SerializeField] private CinemachineVirtualCamera camera;
            public CameraType Type => type;
            [SerializeField] private CameraType type;
        }
    }
}
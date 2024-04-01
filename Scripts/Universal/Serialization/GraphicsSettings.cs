using UnityEngine;
using UnityEngine.Events;

namespace Universal.Serialization
{
    [System.Serializable]
    public class GraphicsSettings
    {
        #region fields & properties
        public SimpleResolution Resolution
        {
            get
            {
                if (resolution.width == 0 || resolution.height == 0)
                {
                    resolution.width = Screen.currentResolution.width;
                    resolution.height = Screen.currentResolution.height;
                }
                
                return resolution;
            }
            set => SetResolution(value);
        }
        [SerializeField] private SimpleResolution resolution;
        public FullScreenMode ScreenMode
        {
            get => screenMode;
            set => SetScreenMode(value);
        }
        [SerializeField] private FullScreenMode screenMode = FullScreenMode.FullScreenWindow;
        public bool Vsync
        {
            get => vsync;
            set => SetVsync(value);
        }
        [SerializeField] private bool vsync = false;
        /// <summary>
        /// 10..inf
        /// </summary>
        public int RefreshRate
        {
            get => refreshRate;
            set => SetRefreshRate(value);
        }
        [SerializeField] private int refreshRate = 60;
        /// <summary>
        /// 0.01..inf
        /// </summary>
        public Vector2 CameraSensitvity
        {
            get => cameraSensitivity;
            set => SetSensitivity(value);
        }
        [SerializeField] private Vector2 cameraSensitivity = Vector2.one;
        /// <summary>
        /// 10..105
        /// </summary>
        public int CameraFOV
        {
            get => cameraFOV;
            set => SetFOV(value);
        }
        [SerializeField] private int cameraFOV = 65;
        #endregion fields & properties

        #region methods
        private void SetFOV(int value)
        {
            value = Mathf.Clamp(value, 10, 105);
            cameraFOV = value;
        }
        private void SetSensitivity(Vector2 value)
        {
            cameraSensitivity = value;
        }
        private void SetResolution(SimpleResolution value)
        {
            resolution = value;
        }
        private void SetScreenMode(FullScreenMode value)
        {
            screenMode = value;
        }
        private void SetVsync(bool value)
        {
            vsync = value;
        }
        private void SetRefreshRate(int value)
        {
            value = Mathf.Max(value, 10);
            refreshRate = value;
        }
        public GraphicsSettings() { }
        public GraphicsSettings(SimpleResolution resolution, FullScreenMode screenMode, bool vsync, int refreshRate, Vector2 cameraSensitvity, int fov)
        {
            Resolution = resolution;
            ScreenMode = screenMode;
            Vsync = vsync;
            RefreshRate = refreshRate;
            CameraSensitvity = cameraSensitvity;
            CameraFOV = fov;
        }
        #endregion methods
    }
}
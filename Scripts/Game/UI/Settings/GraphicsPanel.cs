using Universal.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Collections;

namespace Game.UI.Settings
{
    public class GraphicsPanel : SettingsPanel<GraphicsSettings>
    {
        #region fields & properties
        protected override GraphicsSettings Settings
        {
            get => Context.GraphicsSettings;
            set => Context.GraphicsSettings = value;
        }
        [SerializeField] private ResolutionsItemList resolutionsList;
        [SerializeField] private Slider fpsSlider;
        [SerializeField] private IntItemList screenModeList;
        [SerializeField] private Slider sensitivitySliderY;
        [SerializeField] private Slider sensitivitySliderX;
        [SerializeField] private Slider fovSlider;

        private List<SimpleResolution> Resolutions
        {
            get
            {
                resolutions ??= GetResolutions();
                return resolutions;
            }
        }
        private List<SimpleResolution> resolutions = null;
        private List<int> ScreenModes
        {
            get
            {
                screenModes ??= new() { (int)FullScreenMode.ExclusiveFullScreen, (int)FullScreenMode.FullScreenWindow, (int)FullScreenMode.Windowed };
                return screenModes;
            }
        }
        private List<int> screenModes = null;
        private int MaxFPS
        {
            get
            {
                if (maxFPS == -1)
                    maxFPS = GetMaxFPS();
                return maxFPS;
            }
        }
        private int maxFPS = -1;
        #endregion fields & properties

        #region methods
        public override GraphicsSettings GetNewSettings()
        {
            SimpleResolution resolution = ExposeItemList(resolutionsList);
            FullScreenMode screenMode = (FullScreenMode)ExposeItemList(screenModeList);
            int fps = (int)ExposeSlider(fpsSlider);
            Vector2 sens = new(ExposeSlider(sensitivitySliderX), ExposeSlider(sensitivitySliderY));
            int fov = (int)ExposeSlider(fovSlider);
            GraphicsSettings newSettings = new(resolution, screenMode, false, fps, sens, fov);
            return newSettings;
        }

        public override void UpdateUI()
        {
            fpsSlider.value = Settings.RefreshRate;
            fpsSlider.maxValue = MaxFPS;
            sensitivitySliderY.value = Settings.CameraSensitvity.y;
            sensitivitySliderX.value = Settings.CameraSensitvity.x;
            fovSlider.value = Settings.CameraFOV;

            screenModeList.SetItems(ScreenModes);
            screenModeList.ItemList.ShowAt((int)Settings.ScreenMode);
            resolutionsList.SetItems(Resolutions);
            resolutionsList.ItemList.ShowAt(Settings.Resolution);
        }
        private List<SimpleResolution> GetResolutions()
        {
            List<SimpleResolution> simpleResolutions = new();
            var resolutions = Screen.resolutions;
            foreach (Resolution res in resolutions)
            {
                if (simpleResolutions.Exists(x => x.width == res.width && x.height == res.height)) continue;
                SimpleResolution sRes = new()
                {
                    width = res.width,
                    height = res.height
                };
                simpleResolutions.Add(sRes);
            }
            simpleResolutions = simpleResolutions.OrderBy(x => x.width).ToList();
            return simpleResolutions;
        }
        private int GetMaxFPS() => (int)Screen.resolutions.Max(x => x.refreshRateRatio).value + 1;
        #endregion methods
    }
}
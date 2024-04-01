using EditorCustom.Attributes;
using Game.Serialization.Settings;
using UnityEngine;
using UnityEngine.UI;
using Game.UI.Elements;
using Game.UI.Collections;

namespace Game.UI.Settings
{
    public abstract class SettingsPanel<T> : MonoBehaviour 
    {
        #region fields & properties
        protected abstract T Settings { get; set; }
        protected SettingsData Context => SettingsData.Data;
        [SerializeField] private CustomButton applyButton;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            UpdateUI();
            applyButton.OnClicked += SetNewSettings;
        }
        protected virtual void OnDisable()
        {
            applyButton.OnClicked -= SetNewSettings;
        }
        public abstract T GetNewSettings();
        [SerializedMethod]
        public virtual void SetNewSettings()
        {
            Settings = GetNewSettings();
        }
        public abstract void UpdateUI();
        protected Value ExposeItemList<Value>(TextItemList<Value> list) => list.CurrentPageItems[0].Value;
        protected float ExposeSlider(Slider slider) => slider.value;
        protected bool ExposeCheckbox(CustomCheckbox checkbox) => checkbox.CurrentState;
        #endregion methods
    }
}
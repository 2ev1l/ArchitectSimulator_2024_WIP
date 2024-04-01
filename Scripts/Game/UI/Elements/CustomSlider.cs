using EditorCustom.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.UI.Cursor;
using static Game.UI.Elements.ProgressBar;

namespace Game.UI.Elements
{
    public class CustomSlider : CursorChanger
    {
        #region fields & properties
        [SerializeField][Required] private Slider slider;
        [SerializeField][Required] private TextMeshProUGUI exposedText;
        [SerializeField] ProgressTextFormat textFormat = ProgressTextFormat.PercentDefault;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            slider.onValueChanged.AddListener(UpdateUI);
            UpdateUI(slider.value);
        }
        protected override void OnDisable()
        {
            slider.onValueChanged.RemoveListener(UpdateUI);
            base.OnDisable();
        }
        private void UpdateUI(float newValue)
        {
            string text = GetText(textFormat, newValue, slider.minValue, slider.maxValue);
            exposedText.text = text;
        }
        private void OnValidate()
        {
            try { UpdateUI(slider.value); }
            catch { }
        }
        #endregion methods
    }
}
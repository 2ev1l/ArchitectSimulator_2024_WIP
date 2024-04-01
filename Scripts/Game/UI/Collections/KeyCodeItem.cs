using Game.Serialization.Settings.Input;
using UnityEngine;
using Game.UI.Elements;
using Game.UI.Text;
using Game.Events;
using Universal.Core;
using Universal.UI.Graphics;

namespace Game.UI.Collections
{
    public class KeyCodeItem : DescriptionItem<KeyCodeInfo>
    {
        #region fields & properties
        public CustomButton Button => button;
        [SerializeField] private CustomButton button;
        private bool isSubscribed = false;
        private static Color DuplicateColor
        {
            get
            {
                if (duplicateColor.Equals(Color.black))
                    ColorUtility.TryParseHtmlString("#F4ADAD", out duplicateColor);
                return duplicateColor;
            }
        }
        private static Color duplicateColor = Color.black;
        private static Color defaultColor = Color.white;
        [SerializeField] private GraphicActions graphicActions;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (value?.Description == KeyCodeDescription.OpenSettings) return;
            Subscribe();
        }
        private void OnDisable()
        {
            UnSubscribe();
        }
        private void OnDestroy()
        {
            button.OnClicked = null;
        }
        public void DisableButton()
        {
            button.enabled = false;
            UpdateUI();
        }
        public void EnableButton()
        {
            button.enabled = true;
            button.OnExitEvent?.Invoke();
            UpdateUI();
        }

        private void Subscribe()
        {
            if (isSubscribed) return;
            if (value == null) return;
            isSubscribed = true;
            value.OnKeyCodeChanged += UpdateUI;
        }
        private void UnSubscribe()
        {
            if (value == null) return;
            isSubscribed = false;
            value.OnKeyCodeChanged -= UpdateUI;
        }
        public void UpdateUI() => UpdateUI(value.Key);
        public void UpdateUI(KeyCode newKeyCode)
        {
            Text.text = LanguageLoader.GetTextByKeyCode(newKeyCode);
        }
        public void SetDuplicate(bool isDuplicated)
        {
            if (isDuplicated)
                graphicActions.ChangeColorSettings(0, DuplicateColor);
            else
                graphicActions.ChangeColorSettings(0, defaultColor);
        }
        public override void OnListUpdate(KeyCodeInfo param)
        {
            value = param;
            TextDescription.text = LanguageLoader.GetTextByType(TextType.Menu, value.Description.GetLanguageTextId());
            UpdateUI();
            if (value.Description == KeyCodeDescription.OpenSettings)
            {
                DisableButton();
                return;
            }
            UnSubscribe();
            Subscribe();
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.Settings.Input;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Core;

namespace Game.UI.Overlay
{
    [DisallowMultipleComponent]
    public class KeyText : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private KeyCodeDescription description;
        [SerializeField][Required] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI languageText;
        private KeyCodeInfo Info
        {
            get
            {
                info ??= FindInfo();
                return info;
            }
        }
        private KeyCodeInfo info;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            Info.OnKeyCodeChanged += UpdateUI;
            UpdateUI();
        }
        private void OnDisable()
        {
            Info.OnKeyCodeChanged -= UpdateUI;
        }
        private KeyCodeInfo FindInfo()
        {
            InputController.AllKeys.Exists(x => x.Description.Equals(description), out KeyCodeInfo found);
            return found;
        }
        private void UpdateUI(KeyCode _) => UpdateUI();
        private void UpdateUI()
        {
            text.text = LanguageLoader.GetTextByKeyCode(Info.Key);
            if (languageText != null)
                languageText.text = LanguageLoader.GetTextByType(TextType.Menu, info.Description.GetLanguageTextId());
        }
        private void OnValidate()
        {
            if (text != null) return;
            TryGetComponent<TextMeshProUGUI>(out text);
        }
        #endregion methods
    }
}
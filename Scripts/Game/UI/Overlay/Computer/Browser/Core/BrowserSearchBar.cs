using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser
{
    public class BrowserSearchBar : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private VirtualBrowser browser;
        [SerializeField] private CustomButton button;
        [SerializeField] private TMP_InputField inputField;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            button.OnClicked += DoSearch;
            browser.OnFocusPageChanged += UpdateField;
        }
        private void OnDisable()
        {
            button.OnClicked -= DoSearch;
            browser.OnFocusPageChanged -= UpdateField;
        }
        private void UpdateField(VirtualBrowserPage newPage)
        {
            inputField.text = browser.GetPageAddress(newPage);
        }
        public void DoSearch()
        {
            browser.GoToPageByAddress(inputField.text);
        }
        #endregion methods
    }
}
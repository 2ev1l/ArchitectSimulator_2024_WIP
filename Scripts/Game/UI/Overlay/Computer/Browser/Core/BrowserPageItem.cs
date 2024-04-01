using EditorCustom.Attributes;
using Game.UI.Collections;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Game.UI.Overlay.Computer.Browser.BrowserPageList;

namespace Game.UI.Overlay.Computer.Browser
{
    public class BrowserPageItem : ContextActionsItem<BrowserPageData>
    {
        #region fields & properties
        [SerializeField] private CustomButton closeButton;
        [SerializeField] private CustomButton pageFocusButton;

        [Title("UI")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image background;
        [SerializeField] private Color hiddenColor;
        [SerializeField] private Color focusColor;
        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            base.UpdateUI();
            switch (Context.Page.CurrentState)
            {
                case VirtualBrowserPage.PageState.Focus:
                    background.color = focusColor;
                    pageFocusButton.enabled = false;
                    break;
                case VirtualBrowserPage.PageState.Hidden:
                    background.color = hiddenColor;
                    pageFocusButton.enabled = true;
                    break;
                default: break;
            }
            text.text = $"{Context.Page.GetFullAddress()}";
        }
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            closeButton.OnClicked += ClosePage;
            pageFocusButton.OnClicked += FocusPage;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            closeButton.OnClicked -= ClosePage;
            pageFocusButton.OnClicked -= FocusPage;
        }

        private void ClosePage()
        {
            Context.Browser.ClosePage(Context.Page);
        }
        private void FocusPage()
        {
            Context.Browser.CurrentPage = Context.Page;
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using Game.Animation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static Game.UI.Overlay.Computer.Browser.VirtualBrowserPage;

namespace Game.UI.Overlay.Computer.Browser
{
    /// <summary>
    /// Place this component on the root of page
    /// </summary>
    public class VirtualBrowserPage : VirtualStateMachine<PageState>
    {
        #region fields & properties
        public UnityAction<VirtualBrowserPage, PageState> OnPageStateChanged;

        public VirtualBrowserPage Parent => parent;
        [SerializeField] private VirtualBrowserPage parent;
        public string SubAddress => subAddress;
        [SerializeField] private string subAddress = "";
        [SerializeField] private ScrollPosition resetPosition;

        public bool CanViewPage
        {
            get => canViewPage;
            set => canViewPage = value;
        }
        [SerializeField] private bool canViewPage = true;
        #endregion fields & properties

        #region methods
        protected override void OnStateChanged(PageState newState)
        {
            switch (newState)
            {
                case PageState.Closed: SetClosedValues(); break;
                case PageState.Focus: SetFocusValues(); break;
                case PageState.Hidden: SetHiddenValues(); break;
                default: throw new System.NotImplementedException(newState.ToString());
            }
        }
        public string GetFullAddress()
        {
            string result = $"{subAddress}";
            VirtualBrowserPage currentParent = parent;
            while (currentParent != null)
            {
                result = result.Insert(0, $"{currentParent.subAddress}{VirtualBrowser.AddressSeparator}");
                currentParent = currentParent.Parent;
            }
            return result;
        }
        private void SetClosedValues()
        {
            try { resetPosition.ScrollToImmediate((RectTransform)resetPosition.ScrollRect.transform); }
            catch { }
            gameObject.SetActive(false);
        }
        private void SetFocusValues()
        {
            gameObject.SetActive(true);
        }
        private void SetHiddenValues()
        {
            gameObject.SetActive(false);
        }
        #endregion methods

        public enum PageState
        {
            Closed,
            Focus,
            Hidden
        }
    }
}
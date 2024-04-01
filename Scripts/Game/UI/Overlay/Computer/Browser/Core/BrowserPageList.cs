using Game.UI.Collections;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Game.UI.Overlay.Computer.Browser.BrowserPageList;

namespace Game.UI.Overlay.Computer.Browser
{
    public class BrowserPageList : ContextInfinityList<BrowserPageData>
    {
        #region fields & properties
        [SerializeField] private VirtualBrowser browser;
        private static readonly VirtualBrowserPage.PageState[] updateablePages = new VirtualBrowserPage.PageState[] { VirtualBrowserPage.PageState.Focus, VirtualBrowserPage.PageState.Hidden };
        private List<BrowserPageData> AllPages
        {
            get
            {
                if (allPages == null)
                {
                    allPages = new();
                    foreach (VirtualBrowserPage page in browser.Pages)
                    {
                        allPages.Add(new(page, browser));
                    }
                }
                return allPages;
            }
        }
        private List<BrowserPageData> allPages = null;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            browser.OnFocusPageChanged += UpdateListData;
            browser.OnPageClosed += UpdateListData;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            browser.OnFocusPageChanged -= UpdateListData;
            browser.OnPageClosed -= UpdateListData;
            base.OnDisable();
        }
        private void UpdateListData(VirtualBrowserPage _) => UpdateListData();
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(AllPages.Where(data => updateablePages.Contains(data.Page.CurrentState)), x => x);
        }

        #endregion methods
        public class BrowserPageData
        {
            public VirtualBrowserPage Page { get; }
            public VirtualBrowser Browser { get; }
            public BrowserPageData(VirtualBrowserPage page, VirtualBrowser browser)
            {
                Page = page;
                Browser = browser;
            }
        }
    }
}
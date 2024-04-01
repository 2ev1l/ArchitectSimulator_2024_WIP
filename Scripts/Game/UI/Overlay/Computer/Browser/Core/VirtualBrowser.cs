using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;
using static Game.UI.Overlay.Computer.Browser.VirtualBrowserPage;

namespace Game.UI.Overlay.Computer.Browser
{
    public class VirtualBrowser : VirtualApplication
    {
        #region fields & properties
        public static readonly string StartAddress = "AS";
        public static readonly string AddressSeparator = "/";
        private static readonly int historyIndexingLimit = 20;

        public UnityAction<VirtualBrowserPage> OnFocusPageChanged;
        public UnityAction<VirtualBrowserPage> OnPageClosed;

        public IReadOnlyList<VirtualBrowserPage> Pages => pages;
        [SerializeField] private List<VirtualBrowserPage> pages;
        [SerializeField] private VirtualBrowserPage homePage;
        [SerializeField] private VirtualBrowserPage errorNotFoundPage;

        public VirtualBrowserPage CurrentPage
        {
            get => currentPage;
            set => GoToPage(value);
        }
        private VirtualBrowserPage currentPage = null;
        private List<VirtualBrowserPage> lastPages = new();
        private VirtualBrowserPage[] NonIndexingPages
        {
            get
            {
                nonIndexingPages ??= new[] { homePage, errorNotFoundPage };
                return nonIndexingPages;
            }
        }
        private VirtualBrowserPage[] nonIndexingPages = null;

        #endregion fields & properties

        #region methods
        protected override void OnFirstStartApplication()
        {
            base.OnFirstStartApplication();
            GoToPage(homePage);
#if UNITY_EDITOR
            if (!homePage.CanViewPage)
                Debug.LogError("Home Page View must be enabled");
#endif //UNITY_EDITOR
        }
        protected override void OnCloseApplication()
        {
            base.OnCloseApplication();
            foreach (VirtualBrowserPage page in pages)
            {
                page.TryApplyState(PageState.Closed);
            }
            GoToPage(homePage);
        }
        protected override void OnHideApplication()
        {
            base.OnHideApplication();
        }

        public string GetPageAddress(VirtualBrowserPage page)
        {
            string result = $"{AddressSeparator}{page.SubAddress}";
            VirtualBrowserPage parent = page.Parent;
            while (parent != null)
            {
                result = result.Insert(0, $"{AddressSeparator}{parent.SubAddress}");
                parent = parent.Parent;
            }
            result = result.Insert(0, $"{StartAddress}");
            return result;
        }
        [SerializedMethod]
        public void GoToPageByAddress(string address)
        {
            if (!TryFixAddress(ref address)) return;

            string[] separatedAddress = address.Split(AddressSeparator);
            Stack<VirtualBrowserPage> addressStack = new();
            VirtualBrowserPage foundPage = null;
            VirtualBrowserPage foundParent = null;
            int deep = separatedAddress.Length;
            int deepCounter = 0;
            for (deepCounter = 0; deepCounter < deep; ++deepCounter)
            {
                string currentSubAddress = separatedAddress[deepCounter];
                foundPage = pages.Find(x => x.SubAddress.ToLower().Equals(currentSubAddress.ToLower()) && x.Parent == foundParent);
                if (foundPage == null)
                {
                    GoToPage(errorNotFoundPage);
                    return;
                }
                foundParent = foundPage;
            }
            GoToPage(foundPage);
        }
        private bool TryFixAddress(ref string address)
        {
            int startRemoveCount = StartAddress.Length + AddressSeparator.Length;
            if (address.Length < startRemoveCount)
            {
                GoToPage(errorNotFoundPage);
                return false;
            }
            address = address.Remove(0, startRemoveCount);
            address = address.Replace(" ", "");

            int lastRemoveCound = address.LastIndexOf($"{AddressSeparator}") == address.Length - 1 ? 1 : 0;
            if (address.Length < lastRemoveCound)
            {
                GoToPage(errorNotFoundPage);
                return false;
            }
            address = address.Remove(address.Length - 1, lastRemoveCound);
            return true;
        }

        [SerializedMethod]
        public void GoToPage(VirtualBrowserPage page)
        {
            if (page == null)
            {
                if (homePage != null) GoToPage(homePage);
                return;
            }
            if (currentPage == null)
            {
                currentPage = homePage;
                currentPage.TryApplyState(PageState.Focus);
                OnFocusPageChanged?.Invoke(currentPage);
                return;
            }
            if (!page.TryApplyState(PageState.Focus)) return;
            if (!page.CanViewPage) //required for page OnEnable check calls
            {
                page.TryApplyState(PageState.Closed);
                return;
            }
            if (currentPage.CurrentState != PageState.Closed && page != currentPage)
                currentPage.TryApplyState(PageState.Hidden);

            IndexPage(page);
            currentPage = page;
            OnFocusPageChanged?.Invoke(currentPage);
        }

        private void IndexPage(VirtualBrowserPage page)
        {
            if (page == null || page == currentPage || NonIndexingPages.Contains(page)) return;

            int pagesCount = lastPages.Count;
            if (pagesCount >= historyIndexingLimit)
                lastPages.RemoveAt(0);
            lastPages.Add(page);
        }
        [SerializedMethod]
        public void GoToLastPage()
        {
            int pagesCount = lastPages.Count;
            if (pagesCount == 0)
            {
                GoToPage(homePage);
                return;
            }
            VirtualBrowserPage lastPage = lastPages[pagesCount - 1];
            lastPages.RemoveAt(pagesCount - 1);
            if (NonIndexingPages.Contains(currentPage))
            {
                GoToPage(lastPage);
                return;
            }
            if (pagesCount == 1)
            {
                GoToPage(homePage);
                return;
            }
            //if current page is indexing
            lastPage = lastPages[pagesCount - 2];
            lastPages.RemoveAt(pagesCount - 2);
            GoToPage(lastPage);
        }

        public void ClosePage(VirtualBrowserPage pageToClose)
        {
            pageToClose.TryApplyState(PageState.Closed);
            if (pageToClose == CurrentPage)
            {
                lastPages = lastPages.Where(x => x.CanViewPage && x.CurrentState == PageState.Hidden).ToList();
                GoToLastPage();
            }

            OnPageClosed?.Invoke(pageToClose);
        }
        #endregion methods

#if UNITY_EDITOR
        [Button(nameof(GetAllPagesInChild))]
        private void GetAllPagesInChild()
        {
            Undo.RecordObject(this, "Get all pages in child");
            pages = GetComponentsInChildren<VirtualBrowserPage>(true).ToList();
        }
#endif //UNITY_EDITOR

    }
}
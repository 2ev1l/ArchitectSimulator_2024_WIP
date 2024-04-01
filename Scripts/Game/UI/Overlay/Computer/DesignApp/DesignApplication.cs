using Game.UI.Overlay.Computer.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class DesignApplication : VirtualApplication
    {
        #region fields & properties
        [SerializeField] private FixedUI fixedPanels;
        [SerializeField] private GameObject blueprintHideMask;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (fixedPanels != null)
                fixedPanels.Block();
        }
        private void OnDisable()
        {
            if (fixedPanels != null)
                fixedPanels.Unlock();
        }
        public override void OnViewFocusChanged()
        {
            base.OnViewFocusChanged();
            bool isMain = IsMainVisibleApplication();
            if (isMain == blueprintHideMask.activeSelf)
                blueprintHideMask.SetActive(!isMain);
        }
        #endregion methods
    }
}
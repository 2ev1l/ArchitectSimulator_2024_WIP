using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class AdditionalPanelStateChange : PanelStateChange
    {
        #region fields & properties
        [SerializeField] private CustomButton activateButton;
        [SerializeField] private GameObject activeIndicator;
        #endregion fields & properties

        #region methods
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            activateButton.enabled = !active;
            activeIndicator.SetActive(active);
        }
        #endregion methods
    }
}
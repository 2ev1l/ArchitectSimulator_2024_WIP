using Game.UI.Collections;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class DescriptionItem<T> : ContextActionsItem<T> where T : class
    {
        #region fields & properties
        [SerializeField] private CustomButton activationButton;
        [SerializeField] private GameObject activationIndicator;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            activationButton.OnClicked += SendPanelRequest;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            activationButton.OnClicked -= SendPanelRequest;
        }
        private void SendPanelRequest()
        {
            new DescriptionPanelRequest<T>(this).Send();
        }
        public virtual void SetItemActive(bool active)
        {
            activationButton.enabled = !active;
            if (activationIndicator != null)
                activationIndicator.SetActive(active);
        }
        #endregion methods
    }
}
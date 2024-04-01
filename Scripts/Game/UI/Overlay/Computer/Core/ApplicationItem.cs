using Game.UI.Collections;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Overlay.Computer
{
    public class ApplicationItem : ContextActionsItem<VirtualApplication>
    {
        #region fields & properties
        [SerializeField] private Image imageIcon;
        [SerializeField] private CustomButton showButton;
        [SerializeField] private Color visibleColor = Color.white;
        [SerializeField] private Color hiddenColor = Color.gray;
        #endregion fields & properties

        #region methods
        private void CheckContextState(VirtualApplication.ApplicationState currentState)
        {
            switch (Context.CurrentState)
            {
                case VirtualApplication.ApplicationState.Visible: imageIcon.color = visibleColor; break;
                case VirtualApplication.ApplicationState.Hidden: imageIcon.color = hiddenColor; break;
                default: break;
            }
        }
        protected override void UpdateUI()
        {
            imageIcon.sprite = Context.Icon;
            CheckContextState(Context.CurrentState);
        }
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            showButton.OnClicked += Context.HideOrShowApplication;
            Context.OnCurrentStateChanged += CheckContextState;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            showButton.OnClicked -= Context.HideOrShowApplication;
            Context.OnCurrentStateChanged -= CheckContextState;
        }
        #endregion methods
    }
}
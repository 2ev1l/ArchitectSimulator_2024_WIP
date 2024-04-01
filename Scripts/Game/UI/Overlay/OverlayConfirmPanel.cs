using Game.Events;
using Game.UI.Elements;
using UnityEngine;
using Universal.Events;

namespace Game.UI.Overlay
{
    public class OverlayConfirmPanel : OverlayPanel
    {
        #region fields & properties
        public CustomButton ConfirmButton => confirmButton;
        [SerializeField] private CustomButton confirmButton;
        private bool confirmState = false;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            confirmButton.OnExitEvent?.Invoke();
            base.OnEnable();
            confirmButton.OnClicked += CloseUI;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            confirmButton.OnClicked -= CloseUI;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            confirmButton.OnClicked = null;
            if (CurrentRequest != null)
                ExecuteRequest();
        }
        public override bool CanExecuteRequest(ExecutableRequest request)
        {
            return request is ConfirmRequest;
        }
        public override bool TryExecuteRequest(ExecutableRequest request)
        {
            if (CanExecuteRequest(request))
            {
                ConfirmRequest cr = (ConfirmRequest)request;
                OpenUI(cr);
                CurrentRequest = cr;
                CloseButton.OnClicked += SetRejectState;
                ConfirmButton.OnClicked += SetConfirmCtate;

                CloseButton.OnClicked += ExecuteRequest;
                ConfirmButton.OnClicked += ExecuteRequest;
                return true;
            }
            return false;
        }
        private void SetConfirmCtate()
        {
            confirmState = true;
        }
        private void SetRejectState()
        {
            confirmState = false;
        }

        protected override void ExecuteRequest()
        {
            CloseButton.OnClicked -= SetRejectState;
            ConfirmButton.OnClicked -= SetConfirmCtate;

            CloseButton.OnClicked -= ExecuteRequest;
            ConfirmButton.OnClicked -= ExecuteRequest;
            ConfirmRequest cr = (ConfirmRequest)CurrentRequest;
            if (confirmState)
                cr.Confirm();
            else
                cr.Reject();

            CurrentRequest = null;
        }
        #endregion methods
    }
}
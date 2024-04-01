using Game.Events;
using Game.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Universal.Events;

namespace Game.UI.Overlay
{
    public class OverlayPanel : MonoBehaviour, IRequestExecutor
    {
        #region fields & properties
        public UnityAction OnPanelClosed;

        [SerializeField] private GameObject panel;
        public CustomButton CloseButton => closeButton;
        [SerializeField] private CustomButton closeButton;
        public TextMeshProUGUI HeaderText => headerText;
        [SerializeField] private TextMeshProUGUI headerText;
        public TextMeshProUGUI InfoText => infoText;
        [SerializeField] private TextMeshProUGUI infoText;
        protected InfoRequest CurrentRequest
        {
            get => currentRequest;
            set => currentRequest = value;
        }
        private InfoRequest currentRequest = null;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            closeButton.OnExitEvent?.Invoke();
            closeButton.OnClicked += CloseUI;
        }
        protected virtual void OnDisable()
        {
            closeButton.OnClicked -= CloseUI;
        }
        protected virtual void OnDestroy()
        {
            closeButton.OnClicked = null;
        }
        public virtual bool CanExecuteRequest(ExecutableRequest request)
        {
            return request.GetType() == typeof(InfoRequest);
        }
        public virtual bool TryExecuteRequest(ExecutableRequest request)
        {
            if (CanExecuteRequest(request))
            {
                CurrentRequest = (InfoRequest)request;
                OpenUI(CurrentRequest);
                closeButton.OnClicked += ExecuteRequest;
                return true;
            }
            return false;
        }
        protected virtual void ExecuteRequest()
        {
            closeButton.OnClicked -= ExecuteRequest;
            currentRequest.Close();
        }
        public virtual void OpenUI(InfoRequest request)
        {
            panel.SetActive(true);
            HeaderText.text = request.HeaderInfo;
            InfoText.text = request.MainInfo;
        }
        public virtual void CloseUI()
        {
            panel.SetActive(false);
            OnPanelClosed?.Invoke();
        }

        #endregion methods
    }
}
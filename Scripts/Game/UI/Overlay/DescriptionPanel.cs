using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Events;

namespace Game.UI.Overlay
{
    public abstract class DescriptionPanel<T> : MonoBehaviour, IRequestExecutor where T : class
    {
        #region fields & properties
        /// <exception cref="System.NullReferenceException"></exception>
        protected T Data => data;
        private T data = null;
        private DescriptionItem<T> activeItem = null;
        [SerializeField] private GameObject panel;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            RequestController.Instance.EnableExecution(this);
            UpdateUI(activeItem);
        }
        private void OnDisable()
        {
            RequestController.Instance.DisableExecution(this);
        }
        protected virtual bool CanExecuteRequest(ExecutableRequest request, out DescriptionPanelRequest<T> descriptionRequest)
        {
            descriptionRequest = null;
            if (request is not DescriptionPanelRequest<T> dr) return false;
            descriptionRequest = dr;
            return true;
        }
        public bool TryExecuteRequest(ExecutableRequest request)
        {
            if (!CanExecuteRequest(request, out DescriptionPanelRequest<T> descriptionRequest)) return false;
            UpdateUI(descriptionRequest.Item);
            request.Close();
            return true;
        }
        private void UpdateUI(DescriptionItem<T> item)
        {
            if (activeItem != null)
                activeItem.SetItemActive(false);
            activeItem = item;
            if (activeItem == null)
            {
                panel.SetActive(false);
                return;
            }
            panel.SetActive(true);
            activeItem.SetItemActive(true);
            this.data = item.Context;
            OnUpdateUI();

        }
        /// <summary>
        /// Guarantees that data will not be null
        /// </summary>
        protected abstract void OnUpdateUI();
        private void OnValidate()
        {
            if (panel == gameObject)
                Debug.LogError("Component must be out of the panel");
        }
        #endregion methods
    }
}
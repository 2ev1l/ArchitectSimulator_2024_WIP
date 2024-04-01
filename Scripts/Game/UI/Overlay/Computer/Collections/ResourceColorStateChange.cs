using Game.DataBase;
using Game.UI.Collections;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Universal.Behaviour;
using Universal.Collections.Generic;

namespace Game.UI.Overlay.Computer.Collections
{
    public class ResourceColorStateChange : StateChange, IListUpdater<ResourceColorInfo>
    {
        #region fields & properties
        public System.Action<ResourceColorStateChange> OnStateChangeRequest;
        public ResourceColorInfo Info => info;
        private ResourceColorInfo info;
        [SerializeField] private Image colorImage;
        [SerializeField] private CustomButton button;
        [SerializeField] private GameObject activeUI;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            button.OnClicked += RequestStateChange;
        }
        private void OnDisable()
        {
            button.OnClicked -= RequestStateChange;
        }
        private void RequestStateChange() => OnStateChangeRequest?.Invoke(this);
        public override void SetActive(bool active)
        {
            if (activeUI.activeSelf != active)
                activeUI.SetActive(active);
            button.enabled = !active;
        }
        private void UpdateUI()
        {
            colorImage.color = info.Color.ToColorRGB();
        }
        public void OnListUpdate(ResourceColorInfo param)
        {
            info = param;
            UpdateUI();
        }
        #endregion methods
    }
}
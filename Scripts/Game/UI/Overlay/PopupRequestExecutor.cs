using Game.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.Behaviour;
using Universal.Collections.Generic;
using Universal.Events;

namespace Game.UI.Overlay
{
    public class PopupRequestExecutor : MonoBehaviour, IRequestExecutor
    {
        #region fields & properties
        [SerializeField] private ObjectPool<DestroyablePoolableObject> popupPool;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            RequestController.Instance.EnableExecution(this);
        }
        private void OnDisable()
        {
            RequestController.Instance.DisableExecution(this);
        }

        public bool TryExecuteRequest(ExecutableRequest request)
        {
            if (request is not PopupRequest popupRequest) return false;
            PopupStatsContent popupContent = (PopupStatsContent)popupPool.GetObject();
            popupContent.UpdateUI(popupRequest);
            popupRequest.Close();
            return true;
        }
        #endregion methods

    }
}
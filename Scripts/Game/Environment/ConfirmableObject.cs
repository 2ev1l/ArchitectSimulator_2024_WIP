using Game.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment
{
    public class ConfirmableObject : InfoObject
    {
        #region fields & properties
        public UnityEvent OnConfirmed;
        #endregion fields & properties

        #region methods
        protected override void OnInteract()
        {
            ConfirmRequest confirmRequest = new(OnConfirm, OnReject, NameInfo, DescriptionInfo);
            confirmRequest.Send();
        }
        protected virtual void OnConfirm()
        {
            OnConfirmed?.Invoke();
        }
        #endregion methods
    }
}
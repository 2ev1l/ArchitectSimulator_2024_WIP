using Game.Events;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment
{
    public class InfoObject : InteractableObject
    {
        #region fields & properties
        protected LanguageInfo NameInfo => nameInfo;
        [SerializeField] private LanguageInfo nameInfo;
        protected LanguageInfo DescriptionInfo => descriptionInfo;
        [SerializeField] private LanguageInfo descriptionInfo;
        [Space]
        public UnityEvent OnRejected;
        #endregion fields & properties

        #region methods
        protected override void OnInteract()
        {
            base.OnInteract();
            InfoRequest infoRequest = new(OnReject, nameInfo, descriptionInfo);
            infoRequest.Send();
        }
        protected virtual void OnReject()
        {
            OnRejected?.Invoke();
        }
        #endregion methods
    }
}
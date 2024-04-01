using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;

namespace Game.UI.Overlay
{
    public class ActionStateChange : StateChange
    {
        #region fields & properties
        public UnityEvent OnActiveStateEvent;
        public UnityEvent OnDisabledStateEvent;
        #endregion fields & properties

        #region methods
        public override void SetActive(bool active)
        {
            if (active)
            {
                OnActiveStateEvent?.Invoke();
            }
            else
            {
                OnDisabledStateEvent?.Invoke();
            }
        }
        #endregion methods
    }
}
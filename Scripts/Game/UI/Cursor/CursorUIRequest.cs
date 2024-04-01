using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Events;

namespace Game.UI.Cursor
{
    [System.Serializable]
    public class CursorUIRequest : ExecutableRequest
    {
        #region fields & properties
        private Action OnExecuted;
        public bool DoClickSound => doClickSound;
        [SerializeField] private bool doClickSound = false;
        public CursorState SetState => setState;
        [SerializeField] private CursorState setState = CursorState.Normal;
        #endregion fields & properties

        #region methods
        public override void Close()
        {
            OnExecuted?.Invoke();
        }
        public CursorUIRequest(bool doClickSound, CursorState setState, Action onExecuted)
        {
            this.doClickSound = doClickSound;
            this.setState = setState;
            OnExecuted = onExecuted;
        }
        #endregion methods
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class UIStateChanger : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private int stateId;
        #endregion fields & properties

        #region methods
        public void Change() => ChangeTo(stateId);
        public void ChangeTo(int stateId) => UIStateMachine.Instance.ApplyState(stateId);
        #endregion methods
    }
}
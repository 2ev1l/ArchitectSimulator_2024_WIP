using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI.Overlay.Computer
{
    /// <summary>
    /// Warning, this state machine is using <see cref="object.Equals(object)"/> for compare.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class VirtualStateMachine<TKey> : MonoBehaviour
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - currentState
        /// </summary>
        public UnityAction<TKey> OnCurrentStateChanged;

        public TKey CurrentState
        {
            get => currentState;
            set => TryApplyState(value);
        }
        [SerializeField] private TKey currentState;
        public TKey LastState => lastState;
        [SerializeField] private TKey lastState;
        #endregion fields & properties

        #region methods
        public virtual bool CanApplyState(TKey newState) => !newState.Equals(currentState);
        public bool TryApplyState(TKey newState)
        {
            if (!CanApplyState(newState)) return false;
            ApplyState(newState);
            return true;
        }
        private void ApplyState(TKey newState)
        {
            currentState = newState;
            OnStateChanged(newState);
            OnCurrentStateChanged?.Invoke(currentState);
            lastState = currentState;
        }
        /// <summary>
        /// Simplified call of <see cref="OnCurrentStateChanged"/>
        /// </summary>
        /// <param name="newState"></param>
        protected virtual void OnStateChanged(TKey newState) { }
        #endregion methods
    }
}
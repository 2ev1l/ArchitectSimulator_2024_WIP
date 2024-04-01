using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Universal.Behaviour
{
    [System.Serializable]
    public class StateMachine
    {
        #region fields & properties
        public UnityAction<StateChange> OnStateChanged;
        public IReadOnlyList<StateChange> States => states;
        [SerializeField] protected List<StateChange> states = new();
        public StateChange DefaultState => (states.Count == 0) ? null : states[0];
        public StateChange CurrentState
        {
            get
            {
                TryApplyDefaultState();
                return currentState;
            }
            private set => SetCurrentStateValue(value);
        }
        private StateChange currentState;
        public int CurrentStateId => states.FindIndex(x => x == CurrentState);
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Old states will not be changed. <br></br>
        /// Sets default state for new states
        /// </summary>
        public void ReplaceStates(List<StateChange> newStates)
        {
            newStates.SetElementsTo(states);
            currentState = null;
            TryApplyDefaultState();
        }
        private void SetCurrentStateValue(StateChange value)
        {
            currentState = value;
            int statesCount = states.Count;
            for (int i = 0; i < statesCount; ++i)
            {
                states[i].SetActive(CurrentState == states[i]);
            }
            OnStateChanged?.Invoke(CurrentState);
        }
        public virtual void ApplyDefaultState() => ApplyState(states[0]);
        public bool TryApplyDefaultState()
        {
            if (currentState != null) return false;
            SetCurrentStateValue(DefaultState);
            return true;
        }
        public virtual void TryApplyState(StateChange choosedState)
        {
            if (CurrentState != null && choosedState == CurrentState && CurrentState != states[0]) return;
            ApplyState(choosedState);
        }
        public virtual void TryApplyState(int stateId) => TryApplyState(states[stateId]);
        /// <summary>
        /// Unsafe method. You are strongly recommend to use <see cref="TryApplyState(StateChange)"/>
        /// </summary>
        /// <param name="choosedState"></param>
        public virtual void ApplyState(StateChange choosedState) => CurrentState = choosedState;
        /// <summary>
        /// Unsafe method. You are strongly recommend to use <see cref="TryApplyState(int)"/>
        /// </summary>
        /// <param name="stateId"></param>
        public virtual void ApplyState(int stateId) => CurrentState = states[stateId];
        public StateMachine() { }
        public StateMachine(List<StateChange> states)
        {
            this.states = states;
        }
        #endregion methods
    }
}
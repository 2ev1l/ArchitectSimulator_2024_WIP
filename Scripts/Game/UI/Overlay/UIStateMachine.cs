using Game.Events;
using Game.Serialization.Settings.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay
{
    public class UIStateMachine : DefaultStateMachine
    {
        #region fields & properties
        public static UIStateMachine Instance { get; private set; }
        private StateChange oldState = null;
        [SerializeField] private StateChange settingsState;
        [SerializeField] private List<StateChange> enablePlayerInputStates;
        [SerializeField] private List<StateChange> returnDefaultStates;
        private bool inputWasDisabled = false;
        #endregion fields & properties

        #region methods
        private void SetInstance()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            if (Instance != this)
            {
                Debug.LogError($"Multiple instance of {nameof(UIStateMachine)} is not allowed");
            }
        }
        protected override void OnEnable()
        {
            SetInstance();
            base.Context.OnStateChanged += CheckStateChanged;
            base.OnEnable();
            InputController.OnKeyDown += CheckDownKey;
            CheckStateChanged(Context.CurrentState);
        }
        protected override void OnDisable()
        {
            base.Context.OnStateChanged -= CheckStateChanged;
            base.OnDisable();
            InputController.OnKeyDown -= CheckDownKey;
        }
        private void CheckStateChanged(StateChange newState)
        {
            if (!enablePlayerInputStates.Contains(newState))
                DisablePlayerInput();
            else
                EnablePlayerInput();

            if (newState == Context.DefaultState && oldState == settingsState)
                EnableUIInput();
            if (newState == settingsState)
                DisableUIInput();

            oldState = newState;
        }
        private void CheckDownKey(KeyCodeInfo info)
        {
            if (info.Description.Equals(KeyCodeDescription.OpenSettings))
            {
                if (returnDefaultStates.Contains(Context.CurrentState)) ApplyDefaultState();
                else ApplyState(settingsState);
                return;
            }
            if (info.Description.Equals(((UIStateChange)Context.CurrentState).CloseKey))
            {
                ApplyDefaultState();
                return;
            }

            ApplyStateByKeyCode(info.Description);
        }
        private void ApplyStateByKeyCode(KeyCodeDescription description)
        {
            foreach (UIStateChange state in base.Context.States.Cast<UIStateChange>())
            {
                if (!state.OpenKey.Equals(description)) continue;
                ApplyState(state);
                return;
            }
        }
        private void DisableUIInput() => InputController.Instance.LockUIInput(1);
        private void EnableUIInput() => InputController.Instance.UnlockUIInput(1);

        private void DisablePlayerInput()
        {
            if (inputWasDisabled) return;
            InputController.Instance.LockPlayerInput(1);
            inputWasDisabled = true;
        }
        private void EnablePlayerInput()
        {
            if (!inputWasDisabled) return;
            InputController.Instance.UnlockPlayerInput(1);
            inputWasDisabled = false;
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.World;
using Game.UI.Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Behaviour;
using Universal.Core;
using Universal.Time;

namespace Game.Environment
{
    public class OfficeStateMachine : MonoBehaviour
    {
        #region fields & properties
        private OfficeStateChange CurrentState => (OfficeStateChange)stateMachine.CurrentState;
        [SerializeField] private StateMachine stateMachine = new();
        [SerializeField] private Transform officeEntry;
        private bool isInputLocked = false;
        private Timer enterTimer = new();
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void Enter()
        {
            int officeId = GameData.Data.CompanyData.OfficeData.Id;
            if (!stateMachine.States.Exists(x => ((OfficeStateChange)x).OfficeId == officeId, out StateChange exist))
            {
                InfoRequest.GetErrorRequest(101).Send();
                return;
            }

            LockInput();
            StartCoroutine(ScreenFade.DoCycle());
            enterTimer.OnChangeEnd = delegate
            {
                EnterImmediately(exist);
                UnlockInput();
            };
            enterTimer.Restart(ScreenFade.LastFadingTime);
        }
        private void EnterImmediately(StateChange officeState)
        {
            stateMachine.TryApplyState(officeState);
            Player.Input.Instance.Moving.TeleportToIgnoreLayer(CurrentState.SafePosition.position, Physics.AllLayers);
        }
        [SerializedMethod]
        public void Exit()
        {
            LockInput();
            StartCoroutine(ScreenFade.DoCycle());
            enterTimer.OnChangeEnd = delegate
            {
                ExitImmediately();
                UnlockInput();
            };
            enterTimer.Restart(ScreenFade.LastFadingTime);
        }
        private void ExitImmediately()
        {
            Player.Input.Instance.Moving.TeleportToIgnoreLayer(officeEntry.position, Physics.AllLayers);
        }
        private void LockInput()
        {
            if (isInputLocked) return;
            isInputLocked = true;
            InputController.Instance.LockFullInput(int.MaxValue);
        }
        private void UnlockInput()
        {
            if (!isInputLocked) return;
            isInputLocked = false;
            InputController.Instance.UnlockFullInput(int.MaxValue);
        }
        #endregion methods

#if UNITY_EDITOR
        [Button(nameof(GetAllStatesInParent))]
        private void GetAllStatesInParent()
        {
            List<StateChange> states = transform.parent.GetComponentsInChildren<OfficeStateChange>().Select(x => (StateChange)x).ToList();
            UnityEditor.Undo.RecordObject(this, "Set states");
            stateMachine = new(states);
        }
        private void OnValidate()
        {
            HashSet<StateChange> sameStates = stateMachine.States.FindSame((x, y) => ((OfficeStateChange)x).OfficeId == ((OfficeStateChange)y).OfficeId);
            if (sameStates.Count > 0)
            {
                foreach (OfficeStateChange el in sameStates.Cast<OfficeStateChange>())
                {
                    Debug.LogError($"Same ids [#{el.OfficeId}] in: {el.name}", el);
                }
            }
        }
#endif //UNITY_EDITOR
    }
}
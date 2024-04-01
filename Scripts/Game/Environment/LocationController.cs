using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.World;
using Game.UI.Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;

namespace Game.Environment
{
    public class LocationController : SingleSceneInstance<LocationController>
    {
        #region fields & properties
        public UnityAction<LocationStateChange> OnLocationChanged;
        public LocationStateChange CurrentLocation => (LocationStateChange)locationStates.CurrentState;
        [SerializeField] private StateMachine locationStates = new();
        private Universal.Time.Timer nextLocationTimer = new();
        private bool isInputLocked = false;
        #endregion fields & properties

        #region methods
        protected override void OnAwake()
        {
            base.OnAwake();
#if UNITY_EDITOR
            if (!enabled)
            {
                Debug.Log("Location controller is disabled. Don't forget to turn it on in build", this);
                return;
            }
#endif //UNITY_EDITOR
            int currentLocation = GameData.Data.LocationsData.CurrentLocationId;
            locationStates.TryApplyState(currentLocation);
            SetLocation(locationStates.CurrentState);
        }
        private void OnEnable()
        {
            locationStates.OnStateChanged += SetLocation;
        }
        private void OnDisable()
        {
            locationStates.OnStateChanged -= SetLocation;
        }
        public void MoveToLocation(int locationId)
        {
            StartCoroutine(ScreenFade.DoCycle());
            LockInput();

            nextLocationTimer.OnChangeEnd = delegate
            {
                try
                {
                    locationStates.TryApplyState(locationId);
                    UnlockInput();
                }
                catch
                {
                    Debug.LogError($"Can't change location to {locationId}. Set to default");
                    locationStates.ApplyDefaultState();
                }
            };
            nextLocationTimer.Restart(ScreenFade.LastFadingTime);
        }
        /// <summary>
        /// Probably you should call <see cref="StateMachine.TryApplyState(int)"/> to invoke full actions
        /// </summary>
        /// <param name="s"></param>
        private void SetLocation(StateChange s)
        {
            LocationStateChange newState = (LocationStateChange)s;
            GameData.Data.LocationsData.CurrentLocationId = locationStates.CurrentStateId;
            TryResetPlayerPosition();
            OnLocationChanged?.Invoke(newState);
        }
        [SerializedMethod]
        public void TryResetPlayerPosition() => TryResetPlayerPosition(out _);
        public bool TryResetPlayerPosition(out Vector3 newPosition)
        {
            newPosition = Vector3.zero;
            Transform position = CurrentLocation.DefaultPosition;
            if (position == null)
            {
                InfoRequest.GetErrorRequest(100).Send();
                return false;
            }
            newPosition = position.position;
            Player.Input.Instance.Moving.TeleportToIgnoreLayer(newPosition, Physics.AllLayers);
            return true;
        }
        private void LockInput()
        {
            if (isInputLocked) return;
            InputController.Instance.LockFullInput(int.MaxValue);
            isInputLocked = true;
        }
        private void UnlockInput()
        {
            if (!isInputLocked) return;
            InputController.Instance.UnlockFullInput(int.MaxValue);
            isInputLocked = false;
        }
        #endregion methods
    }
}
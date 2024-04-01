using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Events;

namespace Game.UI.Overlay.Computer.Core
{
    public class FixedUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private VirtualComputer computer;
        [SerializeField] private GameObject ui;
        private ActionRequest enableRequest = new();
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            computer.OnCurrentStateChanged += CheckState;
            CheckState();
        }
        private void OnDisable()
        {
            computer.OnCurrentStateChanged -= CheckState;
        }
        public void Block()
        {
            enableRequest.AddBlockLevel(0);
            CheckState();
        }
        public void Unlock()
        {
            enableRequest.RemoveBlockLevel(0);
            CheckState();
        }
        private void CheckState(VirtualComputer.ComputerState computerState)
        {
            if (computerState == VirtualComputer.ComputerState.Disabled) return;
            CheckState();
        }
        private void CheckState()
        {
            if (ui == null) return;
            bool uiEnabled = enableRequest.CanExecute(0);
            ui.SetActive(uiEnabled);
        }
        #endregion methods
    }
}
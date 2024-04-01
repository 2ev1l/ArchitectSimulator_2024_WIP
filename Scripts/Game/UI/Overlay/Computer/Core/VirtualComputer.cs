using EditorCustom.Attributes;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;
using static Game.UI.Overlay.Computer.VirtualComputer;

namespace Game.UI.Overlay.Computer
{
    public class VirtualComputer : VirtualStateMachine<ComputerState>
    {
        #region fields & properties
        public IReadOnlyList<VirtualApplication> AvailableApplications => availableApplications;
        [SerializeField] private List<VirtualApplication> availableApplications;
        //probably this should be in more specific inheritance, but for now there's no other virtual computers than overlay
        [SerializeField][Min(-1)] private int lastLocationOpened = -1;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (GameData.Data.LocationsData.CurrentLocationId != lastLocationOpened)
                Close();
            TryApplyState(ComputerState.Enabled);
        }
        [SerializedMethod]
        public void Close() => TryApplyState(ComputerState.Disabled);
        protected override void OnStateChanged(ComputerState newState)
        {
            switch (newState)
            {
                case ComputerState.Enabled: lastLocationOpened = GameData.Data.LocationsData.CurrentLocationId; break;
                case ComputerState.Disabled: availableApplications.ForEach(x => x.TryApplyState(VirtualApplication.ApplicationState.Closed)); break;
                default: throw new System.NotImplementedException(newState.ToString());
            }
        }
        #endregion methods
        public enum ComputerState
        {
            Enabled,
            Disabled
        }

#if UNITY_EDITOR
        [Button(nameof(GetAllApplicationsInChild))]
        private void GetAllApplicationsInChild()
        {
            Undo.RecordObject(this, "Get all pages in child");
            availableApplications = GetComponentsInChildren<VirtualApplication>(true).ToList();
        }
#endif //UNITY_EDITOR
    }
}
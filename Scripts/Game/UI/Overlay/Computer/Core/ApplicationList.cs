using Game.UI.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Game.UI.Overlay.Computer.VirtualApplication;

namespace Game.UI.Overlay.Computer
{
    public class ApplicationList : ContextInfinityList<VirtualApplication>
    {
        #region fields & properties
        private static readonly ApplicationState[] VisibleStates = new[] { ApplicationState.Visible, ApplicationState.Hidden };

        [SerializeField] private VirtualComputer computer;
        private List<VirtualApplication> availableApplications = new();
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (VirtualApplication app in computer.AvailableApplications)
            {
                app.OnCurrentStateChanged += CheckNewApplicationState;
            }
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (VirtualApplication app in computer.AvailableApplications)
            {
                app.OnCurrentStateChanged -= CheckNewApplicationState;
            }
        }
        private void CheckNewApplicationState(ApplicationState state)
        {
            foreach (VirtualApplication app in availableApplications)
            {
                app.OnViewFocusChanged();
            }
            if (state == ApplicationState.Hidden) return;
            UpdateListData();
        }
        public override void UpdateListData()
        {
            availableApplications.Clear();
            foreach (VirtualApplication app in computer.AvailableApplications)
            {
                if (!VisibleStates.Contains(app.CurrentState)) continue;
                availableApplications.Add(app);
            }
            ItemList.UpdateListDefault(availableApplications, x => x);
        }
        #endregion methods
    }
}
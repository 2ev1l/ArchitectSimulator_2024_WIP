using DebugStuff;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class FloorsStateMachine : DefaultStateMachine
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            BlueprintEditor.Instance.Creator.OnFloorChanged += CheckState;
            BlueprintEditor.Instance.OnCurrentDataChanged += CheckState;
            CheckState();
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            BlueprintEditor.Instance.Creator.OnFloorChanged -= CheckState;
            BlueprintEditor.Instance.OnCurrentDataChanged -= CheckState;
            base.OnDisable();
        }
        private void CheckState()
        {
            bool canOpen = BlueprintEditor.Instance.CanOpenEditor();

            BuildingFloor currentFloor = BlueprintEditor.Instance.Creator.CurrentBuildingFloor;
            foreach (StateChange state in Context.States)
            {
                FloorStateChange floor = (FloorStateChange)state;
                floor.SetActive(canOpen && floor.AllowedFloors.HasFlag(currentFloor));
            }
        }
        #endregion methods
#if UNITY_EDITOR
        private void OnValidate()
        {
            DebugCommands.CastInterfacesList<FloorStateChange>(Context.States, "States", this);
        }
#endif //UNITY_EDITOR
    }
}
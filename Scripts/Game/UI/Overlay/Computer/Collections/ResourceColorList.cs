using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Collections
{
    /// <summary>
    /// You need to manually invoke data update
    /// </summary>
    public class ResourceColorList : InfinityItemListBase<ResourceColorStateChange, ResourceColorInfo>
    {
        #region fields & properties
        public UnityAction OnStateChanged;
        private ResourceInfo resourceInfo;
        public int CurrentColorId => CurrentState.Id;
        public ResourceColorInfo CurrentState => ((ResourceColorStateChange)stateMachine.CurrentState).Info;
        private readonly StateMachine stateMachine = new();
        private readonly List<StateChange> states = new();
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            foreach (var el in ItemList.Items)
            {
                el.OnStateChangeRequest += ChangeColorState;
            }
            //don't update list data
        }
        protected override void OnDisable()
        {
            foreach (var el in ItemList.Items)
            {
                el.OnStateChangeRequest -= ChangeColorState;
            }
        }
        public void UpdateListData(ResourceInfo newInfo)
        {
            if (newInfo == resourceInfo) return;
            resourceInfo = newInfo;
            UpdateListData();
        }
        /// <summary>
        /// You need <see cref="UpdateListData(ResourceInfo)"/>
        /// </summary>
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(resourceInfo.Prefab.MaterialsInfo, x => x);
            states.Clear();
            foreach (var el in ItemList.Items)
            {
                el.OnStateChangeRequest -= ChangeColorState;
                states.Add(el);
                el.OnStateChangeRequest += ChangeColorState;
            }
            stateMachine.ReplaceStates(states);
        }
        public bool TryApplyState(int resourceColorId)
        {
            if (!ItemList.Items.Exists(x => x.Info.Id == resourceColorId, out ResourceColorStateChange exist)) return false;
            ChangeColorState(exist);
            return true;
        }
        private void ChangeColorState(ResourceColorStateChange state)
        {
            stateMachine.TryApplyState(state);
            OnStateChanged?.Invoke();
        }
        #endregion methods
    }
}
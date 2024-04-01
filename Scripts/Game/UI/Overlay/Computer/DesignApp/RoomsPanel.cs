using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class RoomsPanel : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private GameObject panel;
        [SerializeField][BitMask] private BuildingFloor allowedFloors;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            BlueprintEditor.Instance.Creator.OnFloorChanged += CheckState;
            BlueprintEditor.Instance.OnCurrentDataChanged += CheckState;
            CheckState();
        }
        private void OnDisable()
        {
            BlueprintEditor.Instance.Creator.OnFloorChanged -= CheckState;
            BlueprintEditor.Instance.OnCurrentDataChanged -= CheckState;
        }
        private void CheckState()
        {
            bool canOpen = BlueprintEditor.Instance.CanOpenEditor();

            BuildingFloor currentFloor = BlueprintEditor.Instance.Creator.CurrentBuildingFloor;
            canOpen = canOpen && allowedFloors.HasFlag(currentFloor);
            if (panel.activeSelf != canOpen)
                panel.SetActive(canOpen);
        }
        #endregion methods
    }
}
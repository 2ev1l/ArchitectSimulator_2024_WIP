using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class SelectedNullInfo : FloorStateChange
    {
        #region fields & properties
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            BlueprintEditor.Instance.OnCurrentDataChanged += CheckEditorOpen;
            BlueprintEditor.Instance.Selector.OnSelectedElementChanged += UpdateUI;
            BlueprintEditor.Instance.Creator.OnFloorChanged += UpdateUI;

            UpdateUI();
            CheckEditorOpen();
        }
        protected virtual void OnDisable()
        {
            BlueprintEditor.Instance.OnCurrentDataChanged -= CheckEditorOpen;
            BlueprintEditor.Instance.Selector.OnSelectedElementChanged -= UpdateUI;
            BlueprintEditor.Instance.Creator.OnFloorChanged -= UpdateUI;
        }
        private void CheckEditorOpen()
        {
            bool canOpen = BlueprintEditor.Instance.CanOpenEditor();
            if (!canOpen)
            {
                SetActive(false);
            }
        }
        private void UpdateUI(BlueprintPlacerBase _) => UpdateUI();
        private void UpdateUI()
        {
            SetActive(BlueprintEditor.Instance.Selector.SelectedElement == null && base.AllowedFloors.HasFlag(BlueprintEditor.Instance.Creator.CurrentBuildingFloor));
        }
        #endregion methods
    }
}
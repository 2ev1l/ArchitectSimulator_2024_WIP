using Game.Events;
using Game.Serialization.Settings.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class BlueprintEditorInput : MonoBehaviour
    {
        #region fields & properties
        private BlueprintEditor MainEditor => BlueprintEditor.Instance;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            InputController.OnKeyDown += CheckDownKey;
        }
        private void OnDisable()
        {
            InputController.OnKeyDown -= CheckDownKey;
        }
        private void CheckDownKey(KeyCodeInfo keyCodeInfo)
        {
            BlueprintPlacerBase placer = MainEditor.Selector.SelectedElement;
            if (placer == null) return;
            switch (keyCodeInfo.Description)
            {
                case KeyCodeDescription.DesignMoveUp: placer.TryMoveToCoordinates(placer.Transform.localPosition + Vector3.up * BlueprintEditor.CELL_SIZE, true); break;
                case KeyCodeDescription.DesignMoveDown: placer.TryMoveToCoordinates(placer.Transform.localPosition + Vector3.down * BlueprintEditor.CELL_SIZE, true); break;
                case KeyCodeDescription.DesignMoveRight: placer.TryMoveToCoordinates(placer.Transform.localPosition + Vector3.right * BlueprintEditor.CELL_SIZE, true); break;
                case KeyCodeDescription.DesignMoveLeft: placer.TryMoveToCoordinates(placer.Transform.localPosition + Vector3.left * BlueprintEditor.CELL_SIZE, true); break;
                case KeyCodeDescription.DesignRotate: placer.BlueprintGraphic.Rotate(); break;
                case KeyCodeDescription.DesignDeselect: MainEditor.Selector.DeselectCurrentElement(true); break;
                case KeyCodeDescription.DesignRemove: placer.RemoveBlueprint(); break;
                case KeyCodeDescription.DesignDuplicate: placer.CloneBlueprint(); break;
                case KeyCodeDescription.DesignFocus: MainEditor.FocusToPosition(placer.Transform.localPosition); break;
            }
        }
        #endregion methods
    }
}
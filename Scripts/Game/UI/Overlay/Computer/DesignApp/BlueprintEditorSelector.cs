using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI.Overlay.Computer.DesignApp
{
    [System.Serializable]
    internal class BlueprintEditorSelector
    {
        #region fields & properties
        public UnityAction<BlueprintPlacerBase> OnSelectedElementChanged;
        public BlueprintPlacerBase SelectedElement => selectedElement;
        private BlueprintPlacerBase selectedElement = null;
        #endregion fields & properties

        #region methods
        public void DeselectCurrentElement(bool forceStopMoving = false)
        {
            TrySelectElement(null, forceStopMoving);
        }
        public bool TrySelectElement(BlueprintPlacerBase element, bool forceStopMoving = false)
        {
            if (element == selectedElement) return false;

            if (selectedElement != null)
            {
                if (!forceStopMoving && selectedElement.IsMoving) return false;
            }

            BlueprintPlacerBase oldSelectedElement = selectedElement;
            selectedElement = element;

            if (oldSelectedElement != null)
            {
                oldSelectedElement.OnDeselected();
            }

            if (selectedElement != null)
            {
                selectedElement.transform.SetAsLastSibling();
                selectedElement.OnSelected();
            }
            OnSelectedElementChanged?.Invoke(selectedElement);
            return true;
        }

        #endregion methods
    }
}
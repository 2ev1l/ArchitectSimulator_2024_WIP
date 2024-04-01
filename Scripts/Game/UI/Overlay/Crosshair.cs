using Game.Events;
using Game.Serialization.Settings;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay
{
    public class Crosshair : SingleSceneInstance<Crosshair>
    {
        #region fields & properties
        [SerializeField] private DefaultStateMachine cursorStates;
        [SerializeField] private CrosshairState interactableState;
        [SerializeField] private TextMeshProUGUI interactKeyText;
        [SerializeField] private TextMeshProUGUI interactInfoText;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            InteractableObject.OnCurrentSelectedChanged += CheckNewInteraction;
            CheckNewInteraction(InteractableObject.CurrentSelected);
        }
        private void OnDisable()
        {
            InteractableObject.OnCurrentSelectedChanged -= CheckNewInteraction;
        }
        private void CheckNewInteraction(InteractableObject interactable)
        {
            if (interactable == null)
            {
                cursorStates.ApplyDefaultState();
                return;
            }
            ApplyInteractableState(interactable);
        }
        private void ApplyInteractableState(InteractableObject interactable)
        {
            cursorStates.ApplyState(interactableState);
            interactKeyText.text = LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.PlayerKeys.InteractKey.Key);
            interactInfoText.text = interactable.InteractDescription.Text;
        }
        #endregion methods
    }
}
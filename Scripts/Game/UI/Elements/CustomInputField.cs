using Game.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Elements
{
    public class CustomInputField : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private TMP_InputField inputField;
        private bool isInputLocked = false;
        #endregion fields & properties

        #region methods

        private void OnEnable()
        {
            inputField.onSelect.AddListener(OnStartEdit);
            inputField.onDeselect.AddListener(OnEndEdit);
        }
        private void OnDisable()
        {
            inputField.onSelect.RemoveListener(OnStartEdit);
            inputField.onDeselect.RemoveListener(OnEndEdit);
        }
        private void OnStartEdit(string _, int _1, int _2) => OnStartEdit(_);
        public void OnStartEdit(string text)
        {
            LockInput();
        }
        public void OnEndEdit(string text)
        {
            CancelInvoke(nameof(UnlockInput));
            Invoke(nameof(UnlockInput), Time.deltaTime);
        }
        private void LockInput()
        {
            if (isInputLocked) return;
            isInputLocked = true;
            InputController.Instance.LockFullInput(int.MaxValue);
        }
        private void UnlockInput()
        {
            if (!isInputLocked) return;
            isInputLocked = false;
            InputController.Instance.UnlockFullInput(int.MaxValue);
        }
        #endregion methods
    }
}
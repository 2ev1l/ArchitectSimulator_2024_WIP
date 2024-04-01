using EditorCustom.Attributes;
using Game.Events;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal abstract class SelectedBlueprintInfo<T> : MonoBehaviour where T : BlueprintPlacerBase
    {
        #region fields & properties
        protected T Context => context;
        private T context = null;

        [Title("UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private CustomButton removeButton;
        [SerializeField] private CustomButton addButton;
        [SerializeField] private CustomButton focusButton;
        [SerializeField] private CustomButton rotateButton;
        [SerializeField] private CustomButton rotateResetButton;
        private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            BlueprintEditor.Instance.OnCurrentDataChanged += CheckEditorOpen;

            BlueprintEditor.Instance.Selector.OnSelectedElementChanged += ChangeContext;
            BlueprintEditor.Instance.Creator.OnAnyPlacerAdded += UpdateUI;
            BlueprintEditor.Instance.Creator.OnAnyPlacerRemoved += UpdateUI;
            UpdateUI();
            SubscribeAtContext();
            CheckEditorOpen();
        }
        protected virtual void OnDisable()
        {
            BlueprintEditor.Instance.OnCurrentDataChanged -= CheckEditorOpen;
            BlueprintEditor.Instance.Selector.OnSelectedElementChanged -= ChangeContext;
            BlueprintEditor.Instance.Creator.OnAnyPlacerAdded -= UpdateUI;
            BlueprintEditor.Instance.Creator.OnAnyPlacerRemoved -= UpdateUI;
            UnsubscribeFromContext();
        }
        private void CheckEditorOpen()
        {
            bool canOpen = BlueprintEditor.Instance.CanOpenEditor();
            if (!canOpen)
            {
                context = null;
                UpdateUI();
            }
        }
        protected virtual void OnSubscribe()
        {
            if (removeButton != null)
                removeButton.OnClicked += RemoveElement;
            if (rotateButton != null)
                rotateButton.OnClicked += Rotate;
            if (rotateResetButton != null)
                rotateResetButton.OnClicked += RotateReset;
            if (addButton != null)
                addButton.OnClicked += AddElement;
            if (focusButton != null)
                focusButton.OnClicked += Focus;
        }
        protected virtual void OnUnsubscribe()
        {
            if (removeButton != null)
                removeButton.OnClicked -= RemoveElement;
            if (rotateButton != null)
                rotateButton.OnClicked -= Rotate;
            if (rotateResetButton != null)
                rotateResetButton.OnClicked -= RotateReset;
            if (addButton != null)
                addButton.OnClicked -= AddElement;
            if (focusButton != null)
                focusButton.OnClicked -= Focus;
        }
        private void UpdateUI()
        {
            bool newPanelState = Context != null;
            if (panel.activeSelf != newPanelState)
                panel.SetActive(newPanelState);
            if (Context == null) return;
            OnUpdateVisibleUI();

        }
        /// <summary>
        /// Invokes if <see cref="Context"/> not null
        /// </summary>
        protected virtual void OnUpdateVisibleUI()
        {

        }
        private void Focus()
        {
            BlueprintEditor.Instance.FocusToPosition(Context.Transform.localPosition);
        }
        private void Rotate()
        {
            Context.BlueprintGraphic.Rotate();
        }
        private void RotateReset()
        {
            Context.BlueprintGraphic.ResetRotation();
        }
        protected void RemoveElement() => Context.RemoveBlueprint();
        protected void AddElement() => Context.CloneBlueprint();
        private void ChangeContext(BlueprintPlacerBase checkPlacer)
        {
            if (checkPlacer is not T placer)
            {
                placer = null;
            }
            UnsubscribeFromContext();
            context = placer;
            UpdateUI();
            SubscribeAtContext();
        }
        private void SubscribeAtContext()
        {
            if (Context == null) return;
            if (isSubscribed) return;
            OnSubscribe();
            isSubscribed = true;
        }
        private void UnsubscribeFromContext()
        {
            if (Context == null) return;
            if (!isSubscribed) return;
            OnUnsubscribe();
            isSubscribed = false;
        }
        #endregion methods
    }
}
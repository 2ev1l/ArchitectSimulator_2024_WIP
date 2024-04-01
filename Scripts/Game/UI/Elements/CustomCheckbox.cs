using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.UI.Cursor;

namespace Game.UI.Elements
{
    public class CustomCheckbox : CursorChanger
    {
        #region fields & properties
        public UnityEvent OnActiveState => onActiveState;
        [SerializeField] private UnityEvent onActiveState;
        public UnityEvent OnDisableState => onDisableState;
        [SerializeField] private UnityEvent onDisableState;
        /// <summary>
        /// <see cref="{T0}"/> - newState
        /// </summary>
        public UnityAction<bool> OnStateChanged;

        public UnityEvent OnHoverEvent => onHoverEvent;
        [SerializeField] private UnityEvent onHoverEvent;
        public UnityEvent OnExitEvent => onExitEvent;
        [SerializeField] private UnityEvent onExitEvent;

        [SerializeField] private GameObject activeMark;
        [SerializeField] private bool changeBackground = false;
        [SerializeField][DrawIf(nameof(changeBackground), true)][Required] private Image background;
        [SerializeField][DrawIf(nameof(changeBackground), true)][Required] private Sprite activeTexture;
        [SerializeField][DrawIf(nameof(changeBackground), true)][Required] private Sprite disableTexture;

        public bool CurrentState
        {
            get => currentState;
            set => SetCurrentState(value);
        }
        [SerializeField] private bool currentState = false;
        [SerializeField] private bool invokeActionsOnEnable = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            OnEnter += HoverUI;
            OnExit += ExitUI;
            if (invokeActionsOnEnable)
            {
                SetCurrentState(currentState);
            }
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            OnEnter -= HoverUI;
            OnExit -= ExitUI;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsButtonLeft(eventData)) return;
            base.OnPointerClick(eventData);

            CurrentState = !CurrentState;
        }
        private void SetCurrentState(bool value)
        {
            currentState = value;
            OnStateChanged?.Invoke(CurrentState);

            if (CurrentState) OnActiveState?.Invoke();
            else OnDisableState?.Invoke();
            UpdateUI();
        }
        private void UpdateUI()
        {
            if (activeMark != null)
                activeMark.SetActive(CurrentState);
            if (changeBackground)
                background.sprite = CurrentState ? activeTexture : disableTexture;
        }
        private void HoverUI()
        {
            OnHoverEvent?.Invoke();
        }
        private void ExitUI()
        {
            OnExitEvent?.Invoke();
        }
        #endregion methods
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (activeMark == null) return;
            UpdateUI();
        }
#endif //UNITY_EDITOR
    }
}
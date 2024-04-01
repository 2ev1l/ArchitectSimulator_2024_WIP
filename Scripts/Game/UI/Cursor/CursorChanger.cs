using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Universal.Events;

namespace Game.UI.Cursor
{
    public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        #region fields & properties
        public UnityAction OnEnter;
        public UnityAction OnExit;
        private static CursorLocation lastLocation = CursorLocation.Out;
        private static GameObject lastGameObject = null;
        public bool IsActive
        {
            get => isActive;
            set => ChangeActive(value);
        }
        private bool isActive = true;
        [SerializeField] private bool doRaycastOnly = false;
        private static readonly CursorUIRequest clickSoundRequest = new(true, CursorState.Point, null);
        private static readonly CursorUIRequest pointCursorRequest = new(false, CursorState.Point, null);
        private static readonly CursorUIRequest clickedCursorRequest = new(false, CursorState.Hold, null);
        private static readonly CursorUIRequest normalCursorRequest = new(false, CursorState.Normal, null);
        #endregion fields & properties

        #region methods
        protected virtual void OnDisable()
        {
            if (lastGameObject == gameObject)
                ExitUI();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsActive) return;
            lastGameObject = eventData.pointerEnter;
            lastLocation = CursorLocation.In;
            EnterUI();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsButtonLeft(eventData) || !IsActive) return;
            lastLocation = CursorLocation.In;
            lastGameObject = eventData.pointerEnter;
            if (!doRaycastOnly)
                clickedCursorRequest.Send();
            //CursorSettings.Instance.SetClickedCursor();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsActive) return;
            lastGameObject = eventData.pointerEnter;
            lastLocation = CursorLocation.Out;
            ExitUI();
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsButtonLeft(eventData) || !IsActive) return;
            switch (lastLocation)
            {
                case CursorLocation.Out: ExitUI(); break;
                case CursorLocation.In:
                    if (lastGameObject == gameObject) EnterUI();
                    else ExitUI();
                    break;
                default: ExitUI(); break;
            }
        }
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!IsButtonLeft(eventData) || !IsActive) return;
            if (!doRaycastOnly)
                clickSoundRequest.Send();

            //CursorSettings.Instance.DoClickSound();
            EnterUI();
        }
        protected bool IsButtonLeft(PointerEventData eventData) => eventData.button == PointerEventData.InputButton.Left;
        private void EnterUI()
        {
            if (!doRaycastOnly)
                pointCursorRequest.Send();
                //CursorSettings.Instance.SetPointCursor();
            OnEnter?.Invoke();
        }
        private void ExitUI()
        {
            if (!doRaycastOnly)
                normalCursorRequest.Send();
            //CursorSettings.Instance.SetDefaultCursor();
            OnExit?.Invoke();
        }
        private void ChangeActive(bool value)
        {
            isActive = value;
            ExitUI();
        }
        #endregion methods
    }
}
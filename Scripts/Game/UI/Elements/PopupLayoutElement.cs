using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Universal.Time;

namespace Game.UI.Elements
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(LayoutElement))]
    public class PopupLayoutElement : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private RectTransform referenceRect;
        [SerializeField] private bool changeWidth = true;
        [SerializeField][DrawIf(nameof(changeWidth), true)][Min(0)] private float startWidth = -1;
        [SerializeField] private bool changeHeight = false;
        [SerializeField][DrawIf(nameof(changeHeight), true)][Min(0)] private float startHeight = -1;
        [SerializeField] private bool changeOnEnable = false;
        [SerializeField][DrawIf(nameof(changeOnEnable), true)][Min(0)] private float returnTime = 2f;
        [SerializeField][Min(0)] private float popupTime = 1f;
        [SerializeField][DrawIf(nameof(changeWidth), true)] private ValueTimeChanger widthChanger = new();
        [SerializeField][DrawIf(nameof(changeHeight), true)] private ValueTimeChanger heightChanger = new();
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            if (changeOnEnable)
            {
                HideImmediately();
                Show();
            }
        }
        protected virtual void OnDisable()
        {
            widthChanger.Break();
            heightChanger.Break();
        }
        [Button(nameof(Show))]
        public void Show()
        {
            CancelInvoke(nameof(Hide));
            if (changeHeight)
                RunValueChanger(heightChanger, layoutElement.preferredHeight, startHeight, x => layoutElement.preferredHeight = x);
            if (changeWidth)
                RunValueChanger(widthChanger, layoutElement.preferredWidth, startWidth, x => layoutElement.preferredWidth = x);
            if (changeOnEnable)
                Invoke(nameof(Hide), popupTime + returnTime);
        }
        [Button(nameof(Hide))]
        public void Hide()
        {
            CancelInvoke(nameof(Hide));
            if (changeWidth)
                RunValueChanger(widthChanger, layoutElement.preferredWidth, 0, x => layoutElement.preferredWidth = x);
            if (changeHeight)
                RunValueChanger(heightChanger, layoutElement.preferredHeight, 0, x => layoutElement.preferredHeight = x);
        }
        private void RunValueChanger(ValueTimeChanger vtc, float start, float end, System.Action<float> changeValue, System.Action onEnd = null)
        {
            vtc.SetValues(start, end);
            vtc.SetActions(changeValue, onEnd + delegate { changeValue.Invoke(end); }, () => gameObject == null);
            vtc.Restart(popupTime);
        }
        public void HideImmediately()
        {
            if (changeHeight)
                layoutElement.preferredHeight = 0;
            if (changeWidth)
                layoutElement.preferredWidth = 0;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (layoutElement == null) TryGetComponent(out layoutElement);
            if (referenceRect == null) TryGetComponent(out referenceRect);
            if (startWidth < 0 || startHeight < 0)
            {
                startWidth = referenceRect.rect.width;
                startHeight = referenceRect.rect.height;
            }
        }
#endif //UNITY_EDITOR
        #endregion methods


    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Universal.Time;

namespace Game.Animation
{
    [System.Serializable]
    public class ScrollPosition
    {
        #region fields & properties
        public ScrollRect ScrollRect => scrollRect;
        [SerializeField] private ScrollRect scrollRect;
        public RectTransform Content => content;
        [SerializeField] private RectTransform content;
        [SerializeField] private float changeTime = 1f;
        [SerializeField] private VectorTimeChanger positionChanger = new();
        #endregion fields & properties

        #region methods
        public void ScrollTo(RectTransform target) => ScrollTo(target.localPosition);
        public void ScrollTo(Vector2 targetLocalPosition)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 viewPortLocalPosition = scrollRect.viewport.localPosition;
            Vector2 finalTargetLocalPosition = new(-(viewPortLocalPosition.x + targetLocalPosition.x), -(viewPortLocalPosition.y + targetLocalPosition.y));
            positionChanger.SetValues(content.localPosition, finalTargetLocalPosition);
            positionChanger.SetActions(x => content.localPosition = x, delegate { content.localPosition = finalTargetLocalPosition; });
            positionChanger.Restart(changeTime);
        }
        public void ScrollToImmediate(RectTransform target) => ScrollToImmediate(target.localPosition);
        public void ScrollToImmediate(Vector2 targetLocalPosition)
        {
            Vector2 viewPortLocalPosition = scrollRect.viewport.localPosition;
            Vector2 finalTargetLocalPosition = new(-(viewPortLocalPosition.x + targetLocalPosition.x), -(viewPortLocalPosition.y + targetLocalPosition.y));
            content.localPosition = finalTargetLocalPosition;
        }
        #endregion methods
    }
}
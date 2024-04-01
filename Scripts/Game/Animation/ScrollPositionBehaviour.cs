using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Universal.Time;

namespace Game.Animation
{
    public class ScrollPositionBehaviour : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private ScrollPosition scrollPosition;
        #endregion fields & properties

        #region methods
        public void ScrollTo(RectTransform target) => scrollPosition.ScrollTo(target);
        #endregion methods
    }
}
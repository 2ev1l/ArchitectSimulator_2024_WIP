using Game.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Universal.Behaviour;

namespace Game.UI.Overlay
{
    public class PopupStatsContent : DestroyablePoolableObject
    {
        #region fields & properties
        [SerializeField] private Image indicator;
        [SerializeField] private TextMeshProUGUI gainText;
        [SerializeField] private TextMeshProUGUI totalText;
        protected static Color NegativeColor
        {
            get
            {
                if (negativeColor == Color.black)
                    ColorUtility.TryParseHtmlString("#F4ADAD", out negativeColor);
                return negativeColor;
            }
        }
        protected static Color negativeColor = Color.black;
        protected static Color PositiveColor
        {
            get
            {
                if (positiveColor == Color.black)
                    ColorUtility.TryParseHtmlString("#ADD9F4", out positiveColor);
                return positiveColor;
            }
        }
        protected static Color positiveColor = Color.black;
        #endregion fields & properties

        #region methods
        public virtual void UpdateUI(PopupRequest popupRequest)
        {
            if (popupRequest.ShowOnlyText)
                UpdateTextUI(popupRequest);
            else
                UpdateNumericUI(popupRequest);
        }
        private void UpdateNumericUI(PopupRequest popupRequest)
        {
            bool positive = popupRequest.ValueGain >= 0;
            gainText.color = positive ? PositiveColor : NegativeColor;
            indicator.color = positive ? PositiveColor : NegativeColor;
            totalText.color = positive ? PositiveColor : NegativeColor;
            totalText.text = $"{popupRequest.ValueCurrent - popupRequest.ValueGain}{popupRequest.TextPostfix} => {popupRequest.ValueCurrent}{popupRequest.TextPostfix}";
            gainText.text = $"{(positive ? "+" : "")}{popupRequest.ValueGain}{popupRequest.TextPostfix}";
            indicator.sprite = popupRequest.IndicatorSprite;
        }
        private void UpdateTextUI(PopupRequest popupRequest)
        {
            gainText.color = Color.white;
            indicator.color = Color.white;
            totalText.color = Color.white;
            totalText.text = $"{popupRequest.TextPostfix}";
            gainText.text = $"";
            indicator.sprite = popupRequest.IndicatorSprite;
        }
        #endregion methods
    }
}
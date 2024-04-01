using EditorCustom.Attributes;
using Game.Events;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Behaviour;
using Universal.Time;
using Game.Animation;
using Game.Serialization.Localization;

namespace Game.UI.Overlay
{
    public class SubtitlesContent : DestroyablePoolableObject
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private ValueTimeChanger textChanger = new();
        private string finalText = "";
        [Title("Animation")]
        [SerializeField] private ObjectScale scaleAnimator;
        #endregion fields & properties

        #region methods
        private void ChangeText()
        {
            textChanger.SetValues(0, 1);
            textChanger.SetActions(x =>
            {
                int textRange = (int)Mathf.Lerp(0, finalText.Length, x);
                subtitleText.text = finalText[..textRange];
            },
            delegate { subtitleText.text = finalText; });
            textChanger.Restart(finalText.Length / (TextData.Instance.LoadedData.AverageCharacterReadingPerSecond * 2));
            transform.localScale = new Vector3(1, 0, 1);
            IncreaseScale();
            Invoke(nameof(DecreaseScale), LiveTime - scaleAnimator.ScaleTime);
        }
        private void DecreaseScale()
        {
            scaleAnimator.ScaleTo(new Vector3(1, 0, 1));
        }
        private void IncreaseScale()
        {
            scaleAnimator.ScaleTo(new Vector3(1, 1, 1));
        }
        public void UpdateUI(string text)
        {
            CancelInvoke(nameof(DecreaseScale));
            finalText = text;
            LiveTime = TextData.Instance.LoadedData.GetAverageTextReadTime(finalText);
            ChangeText();
        }
        #endregion methods
    }
}
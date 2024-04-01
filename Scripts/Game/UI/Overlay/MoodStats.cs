using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Universal.Core;

namespace Game.UI.Overlay
{
    public class MoodStats : TextStatsContent
    {
        #region fields & properties
        [SerializeField] private Image image;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            PlayerData.Mood.OnValueChanged += UpdateUI;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            PlayerData.Mood.OnValueChanged -= UpdateUI;
            base.OnDisable();
        }
        private void UpdateUI(int _1, int _2) => UpdateUI();
        public override void UpdateUI()
        {
            int mood = PlayerData.Mood.Value;
            Text.text = $"{mood}%";
            if (DB.Instance == null) return; //can be on disable
            MoodInfo info = DB.Instance.MoodInfo.Data.NearestBottom(x => x.Data.MinMood, mood).Data;
            image.sprite = info.Sprite;
        }
        #endregion methods
    }
}
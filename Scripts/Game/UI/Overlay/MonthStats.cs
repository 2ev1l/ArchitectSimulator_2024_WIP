using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Overlay
{
    public class MonthStats : TextStatsContent
    {
        #region fields & properties
        private static readonly LanguageInfo monthText = new(2, TextType.Game);
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            PlayerData.MonthData.OnMonthChanged += UpdateUI;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            PlayerData.MonthData.OnMonthChanged -= UpdateUI;
            base.OnDisable();
        }
        private void UpdateUI(int _1) => UpdateUI();
        public override void UpdateUI()
        {
            Text.text = $"{monthText.Text} {PlayerData.MonthData.CurrentMonth}";
        }
        #endregion methods
    }
}
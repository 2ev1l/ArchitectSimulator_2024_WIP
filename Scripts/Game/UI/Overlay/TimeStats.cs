using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class TimeStats : TextStatsContent
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            PlayerData.MonthData.FreeTime.OnValueChanged += UpdateUI;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            PlayerData.MonthData.FreeTime.OnValueChanged -= UpdateUI;
            base.OnDisable();
        }
        private void UpdateUI(int _1, int _2) => UpdateUI();
        public override void UpdateUI()
        {
            Text.text = $"{PlayerData.MonthData.FreeTime.Value} h.";
        }
        #endregion methods
    }
}
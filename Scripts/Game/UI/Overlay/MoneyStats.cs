using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class MoneyStats : TextStatsContent
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            PlayerData.Wallet.OnValueChanged += UpdateUI;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            PlayerData.Wallet.OnValueChanged -= UpdateUI;
            base.OnDisable();
        }
        private void UpdateUI(int _1, int _2) => UpdateUI();
        public override void UpdateUI()
        {
            Text.text = $"${PlayerData.Wallet.Value}";
        }
        #endregion methods
    }
}
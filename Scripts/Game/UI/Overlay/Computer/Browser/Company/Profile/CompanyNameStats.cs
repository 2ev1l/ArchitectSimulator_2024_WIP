using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class CompanyNameStats : TextStatsContent
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void UpdateUI()
        {
            Text.text = $"{CompanyData.Name}";
        }
        #endregion methods
    }
}
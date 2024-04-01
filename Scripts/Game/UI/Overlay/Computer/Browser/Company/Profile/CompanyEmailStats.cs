using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class CompanyEmailStats : TextStatsContent
{
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void UpdateUI()
        {
            Text.text = $"{CompanyData.Name}@asmail.com";
        }
        #endregion methods
    }
}
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class CompanyOrdersStats : TextStatsContent
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void UpdateUI()
        {
            Text.text = $"{PlayerData.Tasks.Data.Items.Where(x => x.Info.TaskType == TaskType.Company).Count()}"; //todo ?
        }
        #endregion methods
    }
}
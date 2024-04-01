using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class CompanyValueStats : TextStatsContent
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void UpdateUI()
        {
            int totalSum = PlayerData.Wallet.Value;
            List<TaskData> companyTasks = PlayerData.Tasks.Data.Items.Where(x => x.Info.TaskType == TaskType.Company).ToList();
            totalSum += companyTasks.Sum(x => x.Info.RewardInfo.Rewards.Sum(reward => reward.Type == RewardType.Money ? reward.Value : 0));
            CompanyData companyData = GameData.Data.CompanyData;
            if (companyData.WarehouseData.RentableInfo != null)
                totalSum += companyData.WarehouseData.RentableInfo.Price;
            if (companyData.OfficeData.RentableInfo != null)
                totalSum += companyData.OfficeData.RentableInfo.Price;

            Text.text = $"${totalSum}"; //todo ?
        }
        #endregion methods
    }
}
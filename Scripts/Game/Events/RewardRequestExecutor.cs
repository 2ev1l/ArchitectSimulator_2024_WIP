using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;
using Universal.Core;
using Universal.Events;

namespace Game.Events
{
    public class RewardRequestExecutor : SingleSceneInstance<RewardRequestExecutor>, IRequestExecutor
    {
        #region fields & properties
        public UnityAction<RewardInfo> OnRewardAdded;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            RequestController.Instance.EnableExecution(this);
        }
        private void OnDisable()
        {
            RequestController.Instance.DisableExecution(this);
        }

        public bool TryExecuteRequest(ExecutableRequest request)
        {
            if (request is not RewardRequest rewardRequest) return false;
            foreach (Reward reward in rewardRequest.Reward.Rewards)
            {
                AddReward(reward);
            }
            OnRewardAdded?.Invoke(rewardRequest.Reward);
            rewardRequest.Close();
            return true;
        }
        private void AddReward(Reward reward)
        {
            PlayerData playerData = GameData.Data.PlayerData;
            CompanyData companyData = GameData.Data.CompanyData;
            int rewardValue = reward.Value;
            switch (reward.Type)
            {
                case RewardType.Money:  playerData.Wallet.TryIncreaseValue(rewardValue); break;
                case RewardType.Hours:  playerData.MonthData.FreeTime.TryIncreaseValue(rewardValue); break;
                case RewardType.Mood:   playerData.Mood.TryIncreaseValue(rewardValue); break;
                case RewardType.Rating: companyData.Rating.TryIncreaseValue(rewardValue); break;
                default: throw new System.NotImplementedException(nameof(reward.Type) + " " + reward.Type);
            }
        }
        #endregion methods
    }
}
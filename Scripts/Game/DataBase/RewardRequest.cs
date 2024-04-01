using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Events;

namespace Game.DataBase
{
    [System.Serializable]
    public class RewardRequest : ExecutableRequest
    {
        #region fields & properties
        public RewardInfo Reward => reward;
        [SerializeField] private RewardInfo reward;
        #endregion fields & properties

        #region methods
        public override void Close()
        {
            reward.OnRewardAdded?.Invoke();
        }
        public RewardRequest(RewardInfo reward)
        {
            this.reward = reward;
        }
        #endregion methods
    }
}
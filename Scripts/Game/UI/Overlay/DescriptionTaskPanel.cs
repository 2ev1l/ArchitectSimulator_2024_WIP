using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Universal.Events;

namespace Game.UI.Overlay
{
    public class DescriptionTaskPanel : DescriptionPanel<TaskData>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject descriptionGroup;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private GameObject rewardGroup;
        [SerializeField] private TaskPreviewItemList previewItemList;
        [SerializeField] private GameObject previewGroup;
        #endregion fields & properties

        #region methods
        protected override void OnUpdateUI()
        {
            nameText.text = Data.Info.NameInfo.Text;
            descriptionText.text = Data.Info.DescriptionInfo.Text;
            descriptionGroup.SetActive(!Data.Info.DescriptionInfo.Text.Equals(""));
            rewardText.text = Data.Info.RewardInfo.GetLanguage();
            rewardGroup.SetActive(Data.Info.RewardInfo.Rewards.Count() != 0);
            previewItemList.TaskData = Data;
            previewItemList.UpdateListData();
            previewGroup.SetActive(Data.Info.SpritesInfo.Count() != 0);
        }
        #endregion methods

    }
}
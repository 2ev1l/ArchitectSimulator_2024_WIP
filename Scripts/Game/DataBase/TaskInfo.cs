using EditorCustom.Attributes;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class TaskInfo : DBInfo
    {
        #region fields & properties
        public LanguageInfo NameInfo => nameInfo;
        [Title("UI")][SerializeField] private LanguageInfo nameInfo = new(0, TextType.Task);
        public LanguageInfo DescriptionInfo => descriptionInfo;
        [SerializeField] private LanguageInfo descriptionInfo = new(0, TextType.Task);
        public IEnumerable<Sprite> SpritesInfo => spritesInfo;
        [SerializeField] private Sprite[] spritesInfo;
        
        public RewardInfo RewardInfo => rewardInfo;
        [Title("Settings")][SerializeField] private RewardInfo rewardInfo;
        
        /// <summary>
        /// 0 equals Infinity
        /// </summary>
        public int MonthDuration => monthDuration;
        [SerializeField][Min(0)] private int monthDuration = 1;
        public TaskType TaskType => taskType;
        [SerializeField] private TaskType taskType = TaskType.Player;

        public IEnumerable<int> NextTasksTrigger => nextTasksTrigger;
        [Title("Base Triggers")][SerializeField] private int[] nextTasksTrigger = new int[0];
        public IEnumerable<int> StartSubtitlesTrigger => startSubtitlesTrigger;
        [SerializeField] private int[] startSubtitlesTrigger = new int[0];
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
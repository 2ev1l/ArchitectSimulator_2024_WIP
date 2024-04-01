using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay
{
    public class TaskPreviewItemList : InfinityItemListBase<TaskPreviewItem, Sprite>
    {
        #region fields & properties
        public TaskData TaskData
        {
            get => taskData;
            set => taskData = value;
        }
        private TaskData taskData;
        #endregion fields & properties

        #region methods
        public override void UpdateListData()
        {
            if (taskData == null) return;
            ItemList.UpdateListDefault(taskData.Info.SpritesInfo, x => x);
        }
        #endregion methods
    }
}
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class DescriptionTaskItemList : DescriptionItemList<TaskData>
    {
        #region fields & properties
        [SerializeField] private bool updateCompletedTasks = false;
        [SerializeField] private bool updateCurrentTasks = false;
        #endregion fields & properties

        #region methods
        protected override IEnumerable<TaskData> GetFilteredItems(IEnumerable<TaskData> currentItems)
        {
            if (updateCompletedTasks)
                currentItems = currentItems.Where(x => x.IsCompleted);
            if (updateCurrentTasks)
                currentItems = currentItems.Where(x => !x.IsCompleted);
            return base.GetFilteredItems(currentItems);
        }
        protected override void UpdateCurrentItems(List<TaskData> currentItemsReference)
        {
            currentItemsReference.Clear();
            foreach (TaskData el in GameData.Data.PlayerData.Tasks.Data.Items)
            {
                currentItemsReference.Add(el);
            }
        }
        #endregion methods
    }
}
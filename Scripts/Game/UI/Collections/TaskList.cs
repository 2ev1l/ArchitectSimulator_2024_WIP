using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;
using static Game.UI.Collections.TaskList;

namespace Game.UI.Collections
{
    internal class TaskList : InfinityItemListBase<TaskItem, TaskShortInfo>
    {
        #region fields & properties
        private readonly List<TaskShortInfo> tasks = new();
        private static TasksData Context => GameData.Data.PlayerData.Tasks;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            Context.Data.OnItemAdded += UpdateListData;
            Context.Data.OnItemRemoved += UpdateListData;
            Context.OnTaskCompleted += UpdateListData;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            Context.Data.OnItemAdded -= UpdateListData;
            Context.Data.OnItemRemoved -= UpdateListData;
            Context.OnTaskCompleted -= UpdateListData;
            base.OnDisable();
        }
        private void UpdateListData(TaskData _) => UpdateListData();
        public override void UpdateListData()
        {
            if (DB.Instance == null) return;
            tasks.Clear();
            foreach (TaskData task in Context.Data.Items)
            {
                if (task.IsExpired()) continue;
                tasks.Add(ToShortInfo(task));
            }

            ItemList.UpdateListDefault(tasks, x => x);
        }
        private TaskShortInfo ToShortInfo(TaskData task)
        {
            return new(TasksData.GetInfo(task), task);
        }
        #endregion methods

        internal class TaskShortInfo
        {
            public TaskInfo Info { get; }
            public TaskData Data { get; }

            public TaskShortInfo(TaskInfo info, TaskData data)
            {
                this.Info = info;
                this.Data = data;
            }
        }
    }
}
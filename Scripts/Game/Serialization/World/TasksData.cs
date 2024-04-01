using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class TasksData
    {
        #region fields & properties
        public UnityAction<TaskData> OnTaskCompleted;
        /// <summary>
        /// Don't get items by id <br></br>
        /// Don't remove items <br></br>
        /// Use <see cref="TryStartTask(int)"/> or <see cref="TryCompleteTask(int)"/> instead of inner methods
        /// </summary>
        public UniqueList<TaskData> Data => data;
        [SerializeField] private UniqueList<TaskData> data = new();
        #endregion fields & properties

        #region methods
        public static TaskInfo GetInfo(int taskId) => DB.Instance.TaskInfo[taskId].Data;
        public static TaskInfo GetInfo(TaskData data) => GetInfo(data.Id);
        /// <summary>
        /// Just checks for exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startedTask"></param>
        /// <returns></returns>
        public bool IsTaskStarted(int id, out TaskData startedTask) => Data.Items.Exists(x => x.Id == id, out startedTask);
        public bool TryStartTask(int id)
        {
            TaskData newTask = new(id);
            return Data.TryAddItem(newTask, x => x.Id == id, out _);
        }
        public bool TryCompleteTask(int id)
        {
            if (!Data.Exists(x => x.Id == id, out TaskData found)) return false;
            if (found.TryComplete())
            {
                TaskInfo info = GetInfo(found);
                info.RewardInfo.AddReward();
                foreach (int nextTaskId in info.NextTasksTrigger)
                {
                    Data.TryAddItem(new(info), x => x.Id == nextTaskId, out _);
                }
                OnTaskCompleted?.Invoke(found);
                return true;
            }
            return false;
        }
        public TasksData() { }

        public TasksData(UniqueList<TaskData> tasks)
        {
            this.data = tasks;
        }

        #endregion methods
    }
}
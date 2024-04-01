using Game.DataBase;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class TaskData
    {
        #region fields & properties
        public int Id => id;
        [SerializeField] private int id;
        public int ExpirationMonth => expirationMonth;
        [SerializeField] private int expirationMonth;
        public bool IsCompleted => isCompleted;
        [SerializeField] private bool isCompleted;
        public int CompletionMonth => completionMonth;
        [SerializeField] private int completionMonth;

        private static int CurrentMonth => GameData.Data.PlayerData.MonthData.CurrentMonth;
        public TaskInfo Info
        {
            get
            {
                if (info == null)
                {
                    try { info = DB.Instance.TaskInfo[Id].Data; }
                    catch { info = null; }
                }
                return info;
            }
        }
        [System.NonSerialized] private TaskInfo info;
        #endregion fields & properties

        #region methods
        public bool TryComplete()
        {
            if (isCompleted) return false;
            isCompleted = true;
            completionMonth = CurrentMonth;
            return true;
        }
        public bool IsExpired() => (expirationMonth <= CurrentMonth && expirationMonth != 0) || (isCompleted && completionMonth < CurrentMonth);

        /// <summary>
        /// Collects data just by id. Use this only when game is initialized
        /// </summary>
        /// <param name="id"></param>
        public TaskData(int id)
        {
            this.id = id;
            int duration = Info.MonthDuration;
            this.expirationMonth = duration == 0 ? 0 : CurrentMonth + duration;
            this.isCompleted = false;
            this.completionMonth = 0;
        }
        /// <summary>
        /// You can use this even when game is not running
        /// </summary>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        public TaskData(int id, int duration, int currentMonth)
        {
            this.id = id;
            this.expirationMonth = duration == 0 ? 0 : currentMonth + duration;
            this.isCompleted = false;
            this.completionMonth = 0;
        }
        /// <summary>
        /// Use this only when game is initialized
        /// </summary>
        /// <param name="taskInfo"></param>
        public TaskData(TaskInfo taskInfo)
        {
            this.id = taskInfo.Id;
            int duration = taskInfo.MonthDuration;
            this.expirationMonth = duration == 0 ? 0 : CurrentMonth + duration;
            this.isCompleted = false;
            this.completionMonth = 0;
        }
        #endregion methods
    }
}
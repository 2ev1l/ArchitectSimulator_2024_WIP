using EditorCustom.Attributes;
using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Game.Events
{
    public class TaskChecker : ResultChecker
    {
        #region fields & properties
        private static TasksData Context => GameData.Data.PlayerData.Tasks;

        [SerializeField][Min(0)] private int taskId;
        [SerializeField] private TaskCheckState checkState;
        [SerializeField] private ResultCombineOperator combineOperator = ResultCombineOperator.Or;
        [SerializeField] private bool subscribeAtChange = true;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            if (subscribeAtChange)
            {
                Context.Data.OnItemAdded += Check;
                GameData.Data.PlayerData.MonthData.OnMonthChanged += Check;
                Context.OnTaskCompleted += Check;
            }
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            if (subscribeAtChange)
            {
                Context.Data.OnItemAdded -= Check;
                GameData.Data.PlayerData.MonthData.OnMonthChanged -= Check;
                Context.OnTaskCompleted -= Check;
            }
            base.OnDisable();
        }
        private void Check(int _) => Check();
        private void Check(TaskData _) => Check();

        public override bool GetResult()
        {
            bool result = combineOperator.GetStartResult();
            bool isStarted = Context.IsTaskStarted(taskId, out TaskData startedTask);
            bool isCompleted = startedTask != null && startedTask.IsCompleted;
            bool isExpired = startedTask != null && startedTask.IsExpired();
            if (checkState.HasFlag(TaskCheckState.NotStarted))
            {
                result = combineOperator.Execute(result, !isStarted);
            }
            if (checkState.HasFlag(TaskCheckState.Started))
            {
                result = combineOperator.Execute(result, isStarted);
            }
            if (checkState.HasFlag(TaskCheckState.Completed))
            {
                result = combineOperator.Execute(result, isCompleted);
            }
            if (checkState.HasFlag(TaskCheckState.Expired))
            {
                result = combineOperator.Execute(result, isExpired);
            }
            return result;
        }

        #endregion methods
        [System.Flags]
        private enum TaskCheckState
        {
            NotStarted = 1,
            Started = 2,
            Completed = 4,
            Expired = 8
        }
    }
}
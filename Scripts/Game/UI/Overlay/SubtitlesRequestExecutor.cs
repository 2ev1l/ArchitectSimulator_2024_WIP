using DebugStuff;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Events;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;
using Universal.Collections.Generic;
using Universal.Events;
using Universal.Time;

namespace Game.UI.Overlay
{
    public class SubtitlesRequestExecutor : MonoBehaviour, IRequestExecutor
    {
        #region fields & properties
        [SerializeField] private ObjectPool<DestroyablePoolableObject> subtitlesPool;
        private Queue<int> subtitlesQueue = new();
        private TimeDelay subtitlesDelay = new();
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            RequestController.Instance.EnableExecution(this);
            GameData.Data.PlayerData.Tasks.Data.OnItemAdded += OnTaskStarted;
        }
        private void OnDisable()
        {
            RequestController.Instance.DisableExecution(this);
            GameData.Data.PlayerData.Tasks.Data.OnItemAdded -= OnTaskStarted;
        }
        private void CheckTasksQueue()
        {
            if (!subtitlesDelay.CanActivate) return;
            if (subtitlesQueue.Count == 0) return;
            int currentId = subtitlesQueue.Dequeue();
            if (!TryShowSubtitle(currentId, out SubtitlesContent content)) return;
            subtitlesDelay.Delay = content.LiveTime - 2;
            subtitlesDelay.OnDelayReady = CheckTasksQueue;
            subtitlesDelay.Activate();
        }
        private void OnTaskStarted(TaskData taskData) => EnqueueSubtitiles(taskData.Info.StartSubtitlesTrigger);
        private void EnqueueSubtitiles(IEnumerable<int> subtitlesId)
        {
            foreach (int subtitleId in subtitlesId)
            {
                subtitlesQueue.Enqueue(subtitleId);
            }
            CheckTasksQueue();
        }
        private bool TryShowSubtitle(int id, out SubtitlesContent content)
        {
            content = null;
            if (!GameData.Data.PlayerData.SubtitlesData.TryAddPlayedSubtitle(id)) return false;
            string text = DB.Instance.SubtitleInfo[id].Data.Text;
            content = (SubtitlesContent)subtitlesPool.GetObject();
            content.UpdateUI(text);
            return true;
        }
        public bool TryExecuteRequest(ExecutableRequest request)
        {
            if (request is not SubtitleRequest subtitleRequest) return false;
            EnqueueSubtitiles(subtitleRequest.Ids);
            subtitleRequest.Close();
            return true;
        }
        #endregion methods
    }
}
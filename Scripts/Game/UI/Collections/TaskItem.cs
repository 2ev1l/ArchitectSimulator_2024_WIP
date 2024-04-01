using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic;
using static Game.UI.Collections.TaskList;

namespace Game.UI.Collections
{
    internal class TaskItem : MonoBehaviour, IListUpdater<TaskShortInfo>
    {
        #region fields & properties
        private TaskShortInfo value = null;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CustomCheckbox checkbox;
        [SerializeField] private PopupLayoutElement popup;
        #endregion fields & properties

        #region methods
        public void OnListUpdate(TaskShortInfo param)
        {
            value = param;
            text.text = value.Info.NameInfo.Text;
            checkbox.CurrentState = param.Data.IsCompleted;
            popup.HideImmediately();
            popup.Show();
        }
        #endregion methods
    }
}
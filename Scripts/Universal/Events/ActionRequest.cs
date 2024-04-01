using EditorCustom.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Universal.Events
{
    /// <summary>
    /// Class allows you to make request lock stack from multiple objects without collecting data from them. <br></br>
    /// From: single => many; To: many => single.
    /// </summary>
    [System.Serializable]
    public class ActionRequest
    {
        #region fields & properties
        private readonly Action action;
        [SerializeField][ReadOnly] private List<int> blockLevels = new();
        #endregion fields & properties

        #region methods
        /// <summary>
        /// You should remove block level added manually
        /// </summary>
        /// <param name="level"></param>
        public void AddBlockLevel(int level)
        {
            blockLevels.Add(level);
        }
        public void RemoveBlockLevel(int level)
        {
            blockLevels.Remove(level);
        }
        /// <summary>
        /// Access level must be greater than any block level for execute <br></br>
        /// Exception : accessLevel and max found level == <see cref="int.MinValue"/>
        /// </summary>
        /// <param name="accessLevel"></param>
        /// <returns></returns>
        public bool CanExecute(int accessLevel)
        {
            int levelsCount = blockLevels.Count;
            if (levelsCount == 0) return true;
            int maxLevel = int.MinValue;
            for (int i = 0; i < levelsCount; ++i)
            {
                int blockLevel = blockLevels[i];
                if (blockLevel > maxLevel)
                    maxLevel = blockLevel;
            }
            return accessLevel > maxLevel || maxLevel == int.MinValue;
        }
        public bool TryExecute(int accessLevel)
        {
            if (!CanExecute(accessLevel)) return false;
            action?.Invoke();
            return true;
        }
        public ActionRequest() { }
        public ActionRequest(Action action)
        {
            this.action = action;
        }
        #endregion methods
    }
}
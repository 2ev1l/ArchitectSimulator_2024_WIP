using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Serialization.World
{
    [System.Serializable]
    public abstract class PremiseData
    {
        #region fields & properties
        public UnityAction OnInfoChanged;
        public int Id => id;
        [SerializeField][Min(-1)] private int id = 0;

        /// <exception cref="System.NullReferenceException"></exception>
        public PremiseInfo Info
        {
            get
            {
                if (info == null || info.Id != Id)
                {
                    try { info = GetPremiseInfo(); }
                    catch { info = null; }
                }
                return info;
            }
        }
        [System.NonSerialized] private PremiseInfo info = null;
        #endregion fields & properties

        #region methods
        public abstract bool CanReplaceInfo(int newInfoId);
        public bool TryReplaceInfo(int newInfoId)
        {
            if (!CanReplaceInfo(newInfoId)) return false;
            id = newInfoId;
            _ = Info;
            OnInfoReplaced();
            OnInfoChanged?.Invoke();
            return true;
        }
        /// <summary>
        /// Invokes just before the action
        /// </summary>
        protected abstract void OnInfoReplaced();
        /// <summary>
        /// Override this instead of Info property
        /// </summary>
        /// <returns></returns>
        protected abstract PremiseInfo GetPremiseInfo();
        public PremiseData(int id)
        {
            this.id = id;
        }
        #endregion methods
    }
}
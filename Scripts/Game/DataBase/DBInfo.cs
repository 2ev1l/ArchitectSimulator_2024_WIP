using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class DBInfo
    {
        #region fields & properties
        public int Id
        {
            get => id;
            internal set => id = value;
        }
        [SerializeField][Min(0)] private int id = 0;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Invokes only if class is connected to DBScriptableObject
        /// </summary>
        public virtual void OnValidate() { }
        #endregion methods
    }
}
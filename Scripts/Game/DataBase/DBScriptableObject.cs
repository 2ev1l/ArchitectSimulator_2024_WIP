using System;
using UnityEngine;

namespace Game.DataBase
{
    public abstract class DBScriptableObject<T> : DBScriptableObjectBase where T : DBInfo
    {
        #region fields & properties
        public override int Id => data.Id;
        public T Data => data;
        [SerializeField] private T data;
        #endregion fields & properties

        #region methods

        #endregion methods
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            data.Id = GetObjectId();
            data.OnValidate();
        }
#endif //UNITY_EDITOR
    }
}
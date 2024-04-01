using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class WarehouseInfo : PremiseInfo
    {
        #region fields & properties
        public float Space => space;
        [SerializeField][Min(0)] private float space = 100;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
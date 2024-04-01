using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public abstract class BuyableResource : BuyableObject
    {
        #region fields & properties
        public abstract ResourceInfo ResourceInfo { get; }
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
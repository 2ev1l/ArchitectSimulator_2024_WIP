using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public abstract class RentablePremise : RentableObject
    {
        #region fields & properties
        public abstract PremiseInfo PremiseInfo { get; }
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
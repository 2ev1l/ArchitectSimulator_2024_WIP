using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public abstract class RentableObject : BuyableObject
    {
        #region fields & properties
        public int RentPrice => rentPrice;
        [Title("Rent")][SerializeField][Min(1)] private int rentPrice = 1;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;

namespace Game.DataBase
{
    [System.Serializable]
    public abstract class BuyableObject : DBInfo
    {
        #region fields & properties
        public abstract DBScriptableObjectBase ObjectReference { get; }
        public int Price => price;
        [SerializeField][Min(1)] private int price = 1;
        [SerializeField] private bool doDiscount = true;
        public int MaxDiscount => doDiscount ? maxDiscount : 0;
        [SerializeField][DrawIf(nameof(doDiscount), true)][Range(1, 100)] private int maxDiscount = 50;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// If object haven't discount, returns 0
        /// </summary>
        /// <returns></returns>
        public int GetRandomDiscount() => Random.Range(0, MaxDiscount);
        #endregion methods
    }
}
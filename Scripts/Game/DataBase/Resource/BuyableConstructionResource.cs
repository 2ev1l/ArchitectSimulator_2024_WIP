using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;

namespace Game.DataBase
{
    [System.Serializable]
    public class BuyableConstructionResource : BuyableResource
    {
        #region fields & properties
        public override DBScriptableObjectBase ObjectReference => resourceInfo;
        public override ResourceInfo ResourceInfo => resourceInfo.Data;
        [SerializeField] private ConstructionResourceInfoSO resourceInfo;
        #endregion fields & properties

        #region methods
        
        #endregion methods
    }
}
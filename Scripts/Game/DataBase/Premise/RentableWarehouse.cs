using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class RentableWarehouse : RentablePremise
    {
        #region fields & properties
        public override DBScriptableObjectBase ObjectReference => warehouseInfo;
        public override PremiseInfo PremiseInfo => warehouseInfo.Data;
        [SerializeField] private WarehouseInfoSO warehouseInfo;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
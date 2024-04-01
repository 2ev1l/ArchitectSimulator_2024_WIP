using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public abstract class RentablePremiseData : PremiseData
    {
        #region fields & properties
        /// <exception cref="System.NullReferenceException"></exception>
        public RentablePremise RentableInfo
        {
            get
            {
                if (rentableInfo == null || rentableInfo.Id != Id)
                {
                    try { rentableInfo = GetRentablePremiseInfo(); }
                    catch { rentableInfo = null; }
                }
                return rentableInfo;
            }
        }
        [System.NonSerialized] private RentablePremise rentableInfo = null;
        #endregion fields & properties

        #region methods
        protected override void OnInfoReplaced()
        {
            _ = RentableInfo;
        }
        protected abstract RentablePremise GetRentablePremiseInfo();
        protected override PremiseInfo GetPremiseInfo() => RentableInfo.PremiseInfo;
        protected RentablePremiseData(int id) : base(id) { }
        #endregion methods
    }
}
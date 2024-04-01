using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class RentablePremiseShopItemData : BuyableObjectItemData<RentablePremise>, ICloneable<RentablePremiseShopItemData>
    {
        #region fields & properties
        public RentablePremiseType PremiseType => premiseType;
        [SerializeField] private RentablePremiseType premiseType;
        #endregion fields & properties

        #region methods
        public override void OnPurchase(int count)
        {
            base.OnPurchase(count);
            switch (premiseType)
            {
                case RentablePremiseType.Office: GameData.Data.CompanyData.OfficeData.TryReplaceInfo(Id); break;
                case RentablePremiseType.Warehouse: GameData.Data.CompanyData.WarehouseData.TryReplaceInfo(Id); break;
                default: throw new System.NotImplementedException($"info for {nameof(premiseType)}: {premiseType}");
            }
            //todo for plots
        }
        protected override RentablePremise GetInfo() => premiseType switch
        {
            RentablePremiseType.Office => DB.Instance.RentableOfficeInfo.Find(x => x.Data.PremiseInfo.Id == Id).Data,
            RentablePremiseType.Warehouse => DB.Instance.RentableWarehouseInfo.Find(x => x.Data.PremiseInfo.Id == Id).Data,
            _ => throw new System.NotImplementedException($"info for {nameof(premiseType)}: {premiseType}")
        };

        public RentablePremiseShopItemData Clone() => new(Id, StartPrice, Discount, PremiseType);

        public RentablePremiseShopItemData(int id, int startPrice, int discount, RentablePremiseType type) : base(id, startPrice, discount)
        {
            premiseType = type;
        }
        #endregion methods

    }
}
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class RentableOfficeShopItem : RentablePremiseShopItem
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override bool CanDoInstantBuy()
        {
            return base.CanDoInstantBuy() && GameData.Data.CompanyData.OfficeData.CanReplaceInfo(PremiseInfo.Id);
        }
        protected override bool IsSoldOut()
        {
            return GameData.Data.CompanyData.OfficeData.Id == PremiseInfo.Id;
        }
        #endregion methods
    }
}
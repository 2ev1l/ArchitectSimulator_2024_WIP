using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class BrowserData
    {
        #region fields & properties
        public ResourceShopData ResourceShop => resourceShop;
        [SerializeField] private ResourceShopData resourceShop = new();
        public RentablePremiseShopData PremiseShop => premiseShop;
        [SerializeField] private RentablePremiseShopData premiseShop = new();
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
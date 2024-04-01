using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class MoneyBuyableObject : BuyableObject
    {
        #region fields & properties
        public override PurchaseRequestSender PurchaseRequest => purchaseRequest;
        [Title("Purchase")][SerializeField] private MoneyPurchaseRequestSender purchaseRequest = new();
        public override int Price => price.GetValueWithInflation(Inflation);
        [SerializeField] private Wallet price = new(10);
        public int Inflation => inflation;
        [SerializeField][Min(0)] private int inflation = 0;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
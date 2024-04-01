using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class MoneyHoursBuyableObject : BuyableObject
    {
        #region fields & properties
        public override PurchaseRequestSender PurchaseRequest => purchaseRequest;
        [Title("Purchase")][SerializeField] private MoneyHoursPurchaseRequestSender purchaseRequest = new();
        public override int Price => int.MaxValue;
        [SerializeField] private Wallet moneyPrice = new(10);
        public int Inflation => inflation;
        [SerializeField][Min(0)] private int inflation = 0;
        [Space]
        [SerializeField][Min(0)] private int hoursPrice = 0;
        #endregion fields & properties

        #region methods
        protected override void OnInteract()
        {
            purchaseRequest.SendRequest(PurchaseName.Text, PurchaseDescription.Text, moneyPrice.GetValueWithInflation(inflation), hoursPrice);
        }
        #endregion methods
    }
}
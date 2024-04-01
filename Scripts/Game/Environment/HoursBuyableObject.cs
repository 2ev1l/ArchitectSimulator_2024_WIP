using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class HoursBuyableObject : BuyableObject
    {
        #region fields & properties
        public override PurchaseRequestSender PurchaseRequest => purchaseRequest;
        [Title("Purchase")][SerializeField] private HoursPurchaseRequestSender purchaseRequest = new();
        public override int Price => price;
        [SerializeField][Min(0)] private int price = 30;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
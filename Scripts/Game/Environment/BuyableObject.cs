using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.World;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Events;

namespace Game.Environment
{
    public abstract class BuyableObject : InteractableObject
    {
        #region fields & properties
        public abstract PurchaseRequestSender PurchaseRequest { get; }
        public abstract int Price { get; }

        protected LanguageInfo PurchaseName => purchaseName;
        [Title("UI")] [SerializeField] private LanguageInfo purchaseName = new(0, TextType.Game);
        protected LanguageInfo PurchaseDescription => purchaseDescription;
        [SerializeField] private LanguageInfo purchaseDescription = new(0, TextType.Game);
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            OnInteracted += OnInteract;
            PurchaseRequest.OnConfirmPurchase += OnConfirmedPurchase;
            PurchaseRequest.OnRejectPurchase += OnRejectededPurchase;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            OnInteracted -= OnInteract;
            PurchaseRequest.OnConfirmPurchase -= OnConfirmedPurchase;
            PurchaseRequest.OnRejectPurchase -= OnRejectededPurchase;
            base.OnDisable();
        }
        protected override void OnInteract()
        {
            base.OnInteract();
            PurchaseRequest.SendRequest(purchaseName.Text, purchaseDescription.Text, Price);
        }
        /// <summary>
        /// Simplified call of <see cref="PurchaseRequestSender.OnConfirmPurchase"/>
        /// </summary>
        protected virtual void OnConfirmedPurchase() { }
        /// <summary>
        /// Simplified call of <see cref="PurchaseRequestSender.OnRejectPurchase"/>
        /// </summary>
        protected virtual void OnRejectededPurchase() { }
        #endregion methods
    }
}
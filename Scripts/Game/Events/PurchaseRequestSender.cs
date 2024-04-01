using Game.Serialization.World;
using Game.UI.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Universal.Events;

namespace Game.Events
{
    [System.Serializable]
    public abstract class PurchaseRequestSender : RequestSender
    {
        #region fields & properties
        public UnityEvent OnConfirmPurchaseEvent;
        public UnityAction OnConfirmPurchase;
        public UnityEvent OnRejectPurchaseEvent;
        public UnityAction OnRejectPurchase;
        public static readonly LanguageInfo PriceInfo = new(6, TextType.Game);
        protected int lastPriceRequest = 0;
        protected abstract Predicate<int> CanDecreaseCurrencyValue { get; }
        protected abstract Predicate<int> TryDecreaseCurrencyValue { get; }
        protected abstract string InsufficientCurrencyInfo { get; }
        #endregion fields & properties

        #region methods
        public abstract string GetPriceText(int price);
        public virtual void SendRequest(string purchaseName, string purchaseDescription, int price)
        {
            lastPriceRequest = price;
            bool canBuy = CanDecreaseCurrencyValue.Invoke(price);
            string description = $"{purchaseDescription}\n<size=90%>{GetPriceText(price)}</size>";
            if (!canBuy)
            {
                description += $"\n<size=80%>{InsufficientCurrencyInfo}</size>";
                InfoRequest infoRequest = new(OnRejectedPurchase, purchaseName, description);
                infoRequest.Send();
                return;
            }

            ConfirmRequest buyRequest = new(OnConfirmedCheckPurchase, OnRejectedPurchase, purchaseName, description);
            buyRequest.Send();
        }
        protected virtual void OnConfirmedCheckPurchase()
        {
            if (!TryDecreaseCurrencyValue.Invoke(lastPriceRequest))
            {
                InfoRequest infoRequest = new(OnRejectedPurchase, InsufficientCurrencyInfo, $"{InsufficientCurrencyInfo}\n<size=90%>{GetPriceText(lastPriceRequest)}</size>");
                infoRequest.Send();
                return;
            }
            OnConfirmPurchase?.Invoke();
            OnConfirmPurchaseEvent?.Invoke();
        }
        protected virtual void OnRejectedPurchase()
        {
            OnRejectPurchase?.Invoke();
            OnRejectPurchaseEvent?.Invoke();
        }
        #endregion methods
    }
}
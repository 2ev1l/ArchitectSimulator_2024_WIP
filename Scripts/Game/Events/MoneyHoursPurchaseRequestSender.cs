using Game.Serialization.World;
using Game.UI.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Events;

namespace Game.Events
{
    /// <summary>
    /// Bad inheritance. Probably should have made a class with support for multiple currency types, but that's the only exception for the situation.
    /// </summary>
    [System.Serializable]
    public class MoneyHoursPurchaseRequestSender : PurchaseRequestSender
    {
        #region fields & properties
        protected override Predicate<int> CanDecreaseCurrencyValue => (x) => false;
        protected override Predicate<int> TryDecreaseCurrencyValue => (x) => false;
        protected override string InsufficientCurrencyInfo => MoneyPurchaseRequestSender.InsufficientMoneyInfo.Text + " / " + HoursPurchaseRequestSender.InsufficientHoursInfo.Text;
        private int lastHoursRequest;
        #endregion fields & properties

        #region methods
        public override string GetPriceText(int value) => "";
        public string GetPriceText(int moneyValue, int hoursValue) => $"{PriceInfo.Text}: ${moneyValue}, {hoursValue} h.";
        private bool CanBuy(int moneyValue, int hoursValue)
        {
            PlayerData playerData = GameData.Data.PlayerData;
            return playerData.Wallet.CanDecreaseValue(moneyValue) && playerData.MonthData.FreeTime.CanDecreaseValue(hoursValue);
        }
        public void SendRequest(string purchaseName, string purchaseDescription, int moneyPrice, int hoursPrice)
        {
            lastHoursRequest = hoursPrice;
            lastPriceRequest = moneyPrice;
            string description = $"{purchaseDescription}\n<size=90%>{GetPriceText(moneyPrice, hoursPrice)}</size>";
            if (!CanBuy(moneyPrice, hoursPrice))
            {
                description += $"\n<size=80%>{InsufficientCurrencyInfo}</size>";
                InfoRequest infoRequest = new(OnRejectedPurchase, purchaseName, description);
                infoRequest.Send();
                return;
            }

            ConfirmRequest buyRequest = new(OnConfirmedCheckPurchase, OnRejectedPurchase, purchaseName, description);
            buyRequest.Send();
        }
        protected override void OnConfirmedCheckPurchase()
        {
            if (!CanBuy(lastPriceRequest, lastHoursRequest))
            {
                InfoRequest infoRequest = new(OnRejectedPurchase, InsufficientCurrencyInfo, $"{InsufficientCurrencyInfo}\n<size=90%>{GetPriceText(lastPriceRequest, lastHoursRequest)}</size>");
                infoRequest.Send();
                return;
            }
            PlayerData playerData = GameData.Data.PlayerData;
            playerData.Wallet.TryDecreaseValue(lastPriceRequest);
            playerData.MonthData.FreeTime.TryDecreaseValue(lastHoursRequest);
            OnConfirmPurchase?.Invoke();
            OnConfirmPurchaseEvent?.Invoke();
        }
        /// <summary>
        /// This method is empty. Use <see cref="SendRequest(string, string, int, int)"/> instead
        /// </summary>
        /// <param name="purchaseName"></param>
        /// <param name="purchaseDescription"></param>
        /// <param name="price"></param>
        public override void SendRequest(string purchaseName, string purchaseDescription, int price) { }
        #endregion methods
    }
}
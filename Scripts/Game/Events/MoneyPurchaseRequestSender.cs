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
    public class MoneyPurchaseRequestSender : PurchaseRequestSender
    {
        #region fields & properties
        public static readonly LanguageInfo InsufficientMoneyInfo = new(7, TextType.Game);
        protected override Predicate<int> CanDecreaseCurrencyValue => GameData.Data.PlayerData.Wallet.CanDecreaseValue;
        protected override Predicate<int> TryDecreaseCurrencyValue => GameData.Data.PlayerData.Wallet.TryDecreaseValue;
        protected override string InsufficientCurrencyInfo => InsufficientMoneyInfo.Text;
        #endregion fields & properties

        #region methods
        public override string GetPriceText(int value) => $"{PriceInfo.Text}: ${value}";
        #endregion methods
    }
}
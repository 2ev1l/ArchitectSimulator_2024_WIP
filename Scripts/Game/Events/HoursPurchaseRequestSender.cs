using Game.Events;
using Game.Serialization.World;
using Game.UI.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Events;

namespace Game.Events
{
    [System.Serializable]
    public class HoursPurchaseRequestSender : PurchaseRequestSender
{
        #region fields & properties
        public static readonly LanguageInfo InsufficientHoursInfo = new(10, TextType.Game);
        protected override Predicate<int> CanDecreaseCurrencyValue => GameData.Data.PlayerData.MonthData.FreeTime.CanDecreaseValue;
        protected override Predicate<int> TryDecreaseCurrencyValue => GameData.Data.PlayerData.MonthData.FreeTime.TryDecreaseValue;
        protected override string InsufficientCurrencyInfo => InsufficientHoursInfo.Text;
        #endregion fields & properties

        #region methods
        public override string GetPriceText(int value) => $"{PriceInfo.Text}: {value} h.";
        #endregion methods
    }
}
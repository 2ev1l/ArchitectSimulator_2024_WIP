using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    #region enum
    public enum RewardType
    {
        Money,
        Hours,
        Mood,
        Rating
    }
    #endregion enum

    public static class RewardTypeExtension
    {
        #region methods
        public static string GetValueText(this RewardType type, int value) => type switch
        {
            RewardType.Money => $"${value}",
            RewardType.Hours => $"{value} h.",
            RewardType.Mood => $"{value}%",
            RewardType.Rating => $"{value}%",
            _ => throw new System.NotImplementedException($"reward type language {type}"),
        };
        public static string GetLanguage(this RewardType type) => type switch
        {
            RewardType.Money => "A.S.V.",
            RewardType.Hours => LanguageLoader.GetTextByType(TextType.Game, 3),
            RewardType.Mood => LanguageLoader.GetTextByType(TextType.Game, 4),
            RewardType.Rating => LanguageLoader.GetTextByType(TextType.Game, 5),
            _ => throw new System.NotImplementedException($"reward type language {type}"),
        };
        #endregion methods
    }
}
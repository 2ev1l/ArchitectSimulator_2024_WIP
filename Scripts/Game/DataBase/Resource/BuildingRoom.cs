using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    #region enum
    public enum BuildingRoom
    {
        Unknown,
        Living,
        Kitchen,
        Bathroom,
        Bedroom
    }
    #endregion enum

    public static class BuildingRoomTypeExtension
    {
        #region methods
        public static string ToLanguage(this BuildingRoom type) => type switch
        {
            BuildingRoom.Unknown => LanguageLoader.GetTextByType(TextType.Resource, 14),
            BuildingRoom.Living => LanguageLoader.GetTextByType(TextType.Resource, 16),
            BuildingRoom.Kitchen => LanguageLoader.GetTextByType(TextType.Resource, 17),
            BuildingRoom.Bathroom => LanguageLoader.GetTextByType(TextType.Resource, 18),
            BuildingRoom.Bedroom => LanguageLoader.GetTextByType(TextType.Resource, 19),
            _ => throw new System.NotImplementedException($"language for {type}"),
        };
        #endregion methods
    }
}
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    #region enum
    public enum ConstructionSubtype
    {
        Base,
        CornerIn,
        CornerOut,
        Door,
        Window,
        Staircase
    }
    #endregion enum

    public static class ConstructionSubtypeExtension
    {
        #region methods
        public static string ToLanguage(this ConstructionSubtype subtype) => subtype switch
        {
            ConstructionSubtype.Base => LanguageLoader.GetTextByType(TextType.Resource, 0),
            ConstructionSubtype.CornerIn => LanguageLoader.GetTextByType(TextType.Resource, 12),
            ConstructionSubtype.CornerOut => LanguageLoader.GetTextByType(TextType.Resource, 13),
            ConstructionSubtype.Door => LanguageLoader.GetTextByType(TextType.Resource, 2),
            ConstructionSubtype.Window => LanguageLoader.GetTextByType(TextType.Resource, 1),
            ConstructionSubtype.Staircase => LanguageLoader.GetTextByType(TextType.Resource, 20),
            _ => throw new System.NotImplementedException($"language for {subtype}"),
        };
        #endregion methods
    }
}
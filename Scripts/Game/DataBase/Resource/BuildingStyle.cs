using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    #region enum
    [System.Flags]
    public enum BuildingStyle
    {
        //none = 0
        American = 1,
        European = 2
    }
    #endregion enum

    public static class BuildingStyleExtension
    {
        #region methods
        /// <summary>
        /// Doesn't work with flags <br></br>
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static string ToLanguage(this BuildingStyle bs) => bs switch
        {
            0 => LanguageLoader.GetTextByType(TextType.Resource, 14),
            BuildingStyle.American => LanguageLoader.GetTextByType(TextType.Resource, 7),
            BuildingStyle.European => LanguageLoader.GetTextByType(TextType.Resource, 8),
            _ => throw new System.NotImplementedException($"langauge for {bs}"),
        };
        #endregion methods
    }
}
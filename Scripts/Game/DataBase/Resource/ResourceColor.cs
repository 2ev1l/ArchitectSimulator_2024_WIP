using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    #region enum
    public enum ResourceColor
    {
        White,
        Gray,
        Black,
        Cyan,
        Red,
        Magenta,
        Yellow,
        Green
    }
    #endregion enum

    public static class ResourceColorExtension
    {
        #region methods
        //todo optimize
        public static Color ToColorRGB(this ResourceColor color) => color switch
        {
            ResourceColor.White => Color.white,
            ResourceColor.Gray => new(0.6235f, 0.6235f, 0.6235f),
            ResourceColor.Black => Color.black,
            ResourceColor.Cyan => new(173 / 255f, 243 / 255f, 244 / 255f),
            ResourceColor.Red => new(251 / 255f, 131 / 255f, 144 / 255f),
            ResourceColor.Magenta => new(244 / 255f, 173 / 255f, 220 / 255f),
            ResourceColor.Yellow => new(255 / 255f, 248 / 255f, 153 / 255f),
            ResourceColor.Green => new(153 / 255f, 255 / 255f, 167 / 255f),
            _ => throw new System.NotImplementedException($"resourceColor to color: {color}")
        };
        #endregion methods
    }
}
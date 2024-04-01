using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    #region enum
    [System.Flags]
    public enum BuildingType
    {
        House = 1,
        Apartments = 2,
        Doghouse = 4
    }
    #endregion enum

    public static class BuildingTypeExtension
    {
        #region methods
        /// <summary>
        /// Doesn't work with flags <br></br>
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static List<BuildingRoom> GetAllowedRooms(this BuildingType bt) => bt switch
        {
            0 => new List<BuildingRoom>() { BuildingRoom.Unknown },
            BuildingType.House => new List<BuildingRoom>() { BuildingRoom.Living, BuildingRoom.Kitchen, BuildingRoom.Bathroom, BuildingRoom.Bedroom },
            BuildingType.Apartments => new List<BuildingRoom>() { BuildingRoom.Living, BuildingRoom.Kitchen, BuildingRoom.Bathroom, BuildingRoom.Bedroom },
            BuildingType.Doghouse => new List<BuildingRoom>() { BuildingRoom.Unknown },
            _ => throw new System.NotImplementedException($"allowed rooms for {bt}"),
        };
        /// <summary>
        /// Doesn't work with flags <br></br>
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static string ToLanguage(this BuildingType bt) => bt switch
        {
            0 => LanguageLoader.GetTextByType(TextType.Resource, 14),
            BuildingType.House => LanguageLoader.GetTextByType(TextType.Resource, 9),
            BuildingType.Apartments => LanguageLoader.GetTextByType(TextType.Resource, 10),
            BuildingType.Doghouse => LanguageLoader.GetTextByType(TextType.Resource, 11),
            _ => throw new System.NotImplementedException($"language for {bt}"),
        };
        #endregion methods
    }
}
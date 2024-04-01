using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    #region enum
    [System.Flags]
    public enum BuildingFloor
    {
        F1 = 1,
        F2 = 2,
        /// <summary>
        /// May be end floor
        /// </summary>
        F3_Roof = 4,
        /// <summary>
        /// Start Floor
        /// </summary>
        F1_Flooring = 8,
        /// <summary>
        /// May be end floor
        /// </summary>
        F2_FlooringRoof = 16,
    }
    #endregion enum

    public static class BuildingFloorExtension
    {
        #region methods
        /// <summary>
        /// Doesn't work with flags <br></br>
        /// Returns next floor or 0 if floors is out
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static BuildingFloor GetNextFloor(this BuildingFloor floor) => floor switch
        {
            BuildingFloor.F1_Flooring => BuildingFloor.F1,
            BuildingFloor.F1 => BuildingFloor.F2_FlooringRoof,
            BuildingFloor.F2_FlooringRoof => BuildingFloor.F2,
            BuildingFloor.F2 => BuildingFloor.F3_Roof,
            BuildingFloor.F3_Roof => 0,
            _ => throw new System.NotImplementedException($"{floor}")
        };
        /// <summary>
        /// Doesn't work with flags <br></br>
        /// Returns previous floor or 0 if floors is out 
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static BuildingFloor GetPrevFloor(this BuildingFloor floor) => floor switch
        {
            BuildingFloor.F1_Flooring => 0,
            BuildingFloor.F1 => BuildingFloor.F1_Flooring,
            BuildingFloor.F2_FlooringRoof => BuildingFloor.F1,
            BuildingFloor.F2 => BuildingFloor.F2_FlooringRoof,
            BuildingFloor.F3_Roof => BuildingFloor.F2,
            _ => throw new System.NotImplementedException($"{floor}")
        };

        public static string ToLanguage(this BuildingFloor bf) => bf switch
        {
            BuildingFloor.F1_Flooring => "F1.0",
            BuildingFloor.F1 => "F1.1",
            BuildingFloor.F2_FlooringRoof => "F2.0",
            BuildingFloor.F2 => "F2.1",
            BuildingFloor.F3_Roof => "F3.0",
            _ => bf.ToString()
        };
        #endregion methods
    }
}
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Events
{
    public class LocationChecker : ResultChecker
    {
        #region fields & properties
        [SerializeField][Min(0)] private int locationId = 0;
        #endregion fields & properties

        #region methods
        public override bool GetResult()
        {
            return GameData.Data.LocationsData.CurrentLocationId == locationId;
        }
        #endregion methods
    }
}
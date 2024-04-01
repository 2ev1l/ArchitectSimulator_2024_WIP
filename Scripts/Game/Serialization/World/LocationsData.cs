using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class LocationsData
    {
        #region fields & properties
        public int CurrentLocationId
        {
            get => currentLocationId;
            set => currentLocationId = Mathf.Max(value, 0);
        }
        [SerializeField][Min(0)] private int currentLocationId = 0;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
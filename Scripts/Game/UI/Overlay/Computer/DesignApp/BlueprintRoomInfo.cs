using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    [System.Serializable]
    internal class BlueprintRoomInfo
    {
        #region fields & properties
        public List<BlueprintPointInfo> LoopPoints => loopPoints;
        [SerializeField] private List<BlueprintPointInfo> loopPoints = new(10);
        public BlueprintPointInfo PointCreated => pointCreated;
        [SerializeField] private BlueprintPointInfo pointCreated;
        #endregion fields & properties

        #region methods
        public BlueprintRoomInfo(BlueprintPointInfo pointCreated)
        {
            this.pointCreated = pointCreated;
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class FloorStateChange : PanelStateChange
    {
        #region fields & properties
        public BuildingFloor AllowedFloors => allowedFloors;
        [SerializeField][BitMask] private BuildingFloor allowedFloors;
        #endregion fields & properties

        #region methods
        
        #endregion methods
    }
}
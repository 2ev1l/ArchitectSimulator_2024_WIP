using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    [System.Serializable]
    internal class BlueprintPointInfo
    {
        #region fields & properties
        public readonly HashSet<BlueprintResourcePlacer> AdjacentElements = new(3);
        public readonly HashSet<BlueprintPointInfo> AdjacentPoints = new(3);
        public readonly HashSet<BlueprintPointInfo> UsedAdjacentPoints = new(3);
        public readonly Stack<BlueprintPointInfo> SubUsedAdjacentPoints = new(3);

        /// <summary>
        /// Actually this element contains in <see cref="AdjacentElements"/> <br></br>
        /// Don't modify from outside. Made as field to increase performance.
        /// </summary>
        public BlueprintResourcePlacer CreatedElement = null;
        /// <summary>
        /// Local Coordinates (for main workflow as a parent) <br></br>
        /// Don't modify from outside. Made as field to increase performance.
        /// </summary>
        public Vector2 LocalWorkflowCoordinates;
        /// <summary>
        /// Don't modify from outside. Made as field to increase performance.
        /// </summary>
        public ConnectedPoint ConnectedCoordinates;
        #endregion fields & properties

        #region methods
        public void TryRecreate(ConnectedPoint connectedCoordinates, BlueprintResourcePlacer createdElement)
        {
            if (this.ConnectedCoordinates == null && connectedCoordinates != null)
            {
                this.ConnectedCoordinates = connectedCoordinates;
            }
            if (this.CreatedElement == null && createdElement != null)
            {
                this.CreatedElement = createdElement;
            }
        }
        public void RecreateWithoutClearingSets(ConnectedPoint connectedCoordinates, Vector2 localWorkflowCoordinates, BlueprintResourcePlacer createdElement)
        {
            //this.coordinates = localWorkflowCoordinates;
            this.ConnectedCoordinates = connectedCoordinates;
            this.CreatedElement = createdElement;
            this.LocalWorkflowCoordinates = localWorkflowCoordinates;
        }
        public void Recreate(ConnectedPoint connectedCoordinates, Vector2 localWorkflowCoordinates, BlueprintResourcePlacer createdElement)
        {
            RecreateWithoutClearingSets(connectedCoordinates, localWorkflowCoordinates, createdElement);
            AdjacentElements.Clear();
            AdjacentPoints.Clear();
            UsedAdjacentPoints.Clear();
            SubUsedAdjacentPoints.Clear();
        }
        public BlueprintPointInfo(ConnectedPoint connectedCoordinates, Vector2 localWorkflowCoordinates, BlueprintResourcePlacer createdElement)
        {
            Recreate(connectedCoordinates, localWorkflowCoordinates, createdElement);
        }
        #endregion methods
    }
}
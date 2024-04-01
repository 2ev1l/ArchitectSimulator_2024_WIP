using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class ConnectedPoint
    {
        #region fields & properties
        /// <summary>
        /// Local Coordinates (for controlled element as a parent)
        /// </summary>
        public Vector2 LocalCoordinates
        {
            get => localCoordinates;
            set => localCoordinates = value;
        }
        [SerializeField] private Vector2 localCoordinates;
        /// <summary>
        /// Local Coordinates (for controlled worfklow as a parent)
        /// </summary>
        public Vector2 LocalWorfklowCoordinates
        {
            get => localWorfklowCoordinates;
            set => localWorfklowCoordinates = value;
        }
        [SerializeField] private Vector2 localWorfklowCoordinates;
        public ConnectedPoint Connected
        {
            get => connected;
            set => connected = value;
        }
        [SerializeField] private ConnectedPoint connected;
        public readonly ConstructionLocation PointLocation;
        #endregion fields & properties

        #region methods
        public ConnectedPoint(Vector2 parentCoordinates, Vector2 workflowCoordinates, ConstructionLocation pointLocation)
        {
            this.localCoordinates = parentCoordinates;
            this.localWorfklowCoordinates = workflowCoordinates;
            this.PointLocation = pointLocation;
        }
        #endregion methods
    }
}
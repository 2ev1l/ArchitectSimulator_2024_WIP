using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;

namespace Game.DataBase
{
    /// <summary>
    /// Supports only +-90 degrees placement
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class RectLine : MonoBehaviour
    {
        #region fields & properties
        /// <summary>
        /// Unmarshalled gameObject for performance increase;
        /// </summary>
        public GameObject GameObject
        {
            get
            {
                if (_gameObject == null)
                    _gameObject = gameObject;
                return _gameObject;
            }
        }
        private GameObject _gameObject = null;
        /// <summary>
        /// Unmarshalled transform for performance increase;
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }
        private Transform _transform = null;
        public RectTransform RectTransform => rectTransform;
        [SerializeField] private RectTransform rectTransform = null;
        public bool Vertical => vertical;
        [SerializeField] private bool vertical = false;
        public ConstructionLocation LineLocation => lineLocation;
        [SerializeField] private ConstructionLocation lineLocation;
        public ConnectedPoint StartPoint
        {
            get
            {
                startPoint ??= new(Vector2.zero, Vector2.zero, LineLocation);
                return startPoint;
            }
        }
        private ConnectedPoint startPoint;
        public ConnectedPoint EndPoint
        {
            get
            {
                endPoint ??= new(Vector2.zero, Vector2.zero, LineLocation);
                return endPoint;
            }
        }
        private ConnectedPoint endPoint;
        #endregion fields & properties

        #region methods
        public void UpdateCorners(Vector2 start, Vector2 end, Vector2 localWorkflowStart, Vector2 localWorkflowEnd)
        {
            StartPoint.Connected = EndPoint;
            EndPoint.Connected = StartPoint;
            StartPoint.LocalCoordinates = start;
            EndPoint.LocalCoordinates = end;
            StartPoint.LocalWorfklowCoordinates = localWorkflowStart;
            EndPoint.LocalWorfklowCoordinates = localWorkflowEnd;
        }
        private void OnValidate()
        {
            if (rectTransform == null || rectTransform.gameObject != this.gameObject)
                rectTransform = GetComponent<RectTransform>();
        }
        #endregion methods
    }
}
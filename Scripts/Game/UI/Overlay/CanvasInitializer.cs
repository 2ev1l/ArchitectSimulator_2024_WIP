using UnityEngine;
using Universal.Core;

namespace Game.UI.Overlay
{
    public class CanvasInitializer : MonoBehaviour, IStartInitializable
    {
        #region fields & properties
        public static Camera OverlayCamera
        {
            get
            {
                if (overlayCamera == null)
                    overlayCamera = GameObject.Find(OverlayCameraName).GetComponent<Camera>();
                return overlayCamera;
            }
        }
        private static Camera overlayCamera;
        public static readonly string OverlayCameraName = "Overlay Camera";
        
        [SerializeField] private Canvas canvas;
        #endregion fields & properties

        #region methods
        public void Start()
        {
            canvas.worldCamera = OverlayCamera;
        }
        #endregion methods
    }
}
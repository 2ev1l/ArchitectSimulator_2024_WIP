using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class TargetCameraLook : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform lookAt;
        #endregion fields & properties

        #region methods
        public void Look()
        {
            CinemachineCamerasController.Instance.FollowTarget = followTarget;
            CinemachineCamerasController.Instance.LookAt = lookAt;
            CinemachineCamerasController.Instance.ChangeCamera(CinemachineCamerasController.CameraType.InstantTargetLook);
        }
        #endregion methods
    }
}
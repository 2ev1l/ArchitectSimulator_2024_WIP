using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.Environment
{
    public class OfficeStateChange : StateChange
    {
        #region fields & properties
        [SerializeField] private GameObject office;
        public Transform SafePosition => safePosition;
        [SerializeField] private Transform safePosition;
        public int OfficeId => officeId;
        [SerializeField][Min(0)] private int officeId = 0;
        #endregion fields & properties

        #region methods
        public override void SetActive(bool active)
        {
            office.SetActive(active);
        }
        #endregion methods
    }
}
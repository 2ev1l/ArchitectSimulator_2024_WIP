using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay
{
    public class CrosshairState : StateChange
    {
        #region fields & properties
        [SerializeField] private List<GameObject> stateObjects;
        #endregion fields & properties

        #region methods
        public override void SetActive(bool active)
        {
            stateObjects.ForEach(x => x.SetActive(active));
        }
        #endregion methods
    }
}
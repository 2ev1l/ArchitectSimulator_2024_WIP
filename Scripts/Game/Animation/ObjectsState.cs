using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Animation
{
    public class ObjectsState : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private List<GameObject> objects;
        [SerializeField] private bool currentState = false;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void FlipState()
        {
            ChangeObjectsState(!currentState);
        }
        [SerializedMethod]
        public void EnableObjects() => ChangeObjectsState(true);
        [SerializedMethod]
        public void DisableObjects() => ChangeObjectsState(false);
        private void ChangeObjectsState(bool state)
        {
            currentState = state;
            objects.ForEach(x => x.SetActive(state));
        }
        #endregion methods
    }
}
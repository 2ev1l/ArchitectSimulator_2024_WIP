using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.Settings.Input
{
    [System.Serializable]
    public class PlayerKeys : KeysCollection
    {
        #region fields & properties
        public KeyCodeInfo MoveForwardKey => moveForwardKey;
        [SerializeField] private KeyCodeInfo moveForwardKey = new(KeyCode.W, KeyCodeDescription.MoveForward);
        public KeyCodeInfo MoveBackwardKey => moveBackwardKey;
        [SerializeField] private KeyCodeInfo moveBackwardKey = new(KeyCode.S, KeyCodeDescription.MoveBackward);
        public KeyCodeInfo MoveRightKey => moveRightKey;
        [SerializeField] private KeyCodeInfo moveRightKey = new(KeyCode.D, KeyCodeDescription.MoveRight);
        public KeyCodeInfo MoveLeftKey => moveLeftKey;
        [SerializeField] private KeyCodeInfo moveLeftKey = new(KeyCode.A, KeyCodeDescription.MoveLeft);
        public KeyCodeInfo InteractKey => interactKey;
        [SerializeField] private KeyCodeInfo interactKey = new(KeyCode.E, KeyCodeDescription.Interact);
        public KeyCodeInfo RunKey => runKey;
        [SerializeField] private KeyCodeInfo runKey = new(KeyCode.LeftShift, KeyCodeDescription.Run);
        #endregion fields & properties

        #region methods
        public override List<KeyCodeInfo> GetKeys()
        {
            List<KeyCodeInfo> list = new()
            {
                MoveForwardKey,
                MoveBackwardKey,
                MoveRightKey,
                MoveLeftKey,
                InteractKey,
                RunKey,
            };
            return list;
        }
        #endregion methods
    }
}
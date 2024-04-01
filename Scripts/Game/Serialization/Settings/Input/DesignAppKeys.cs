using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.Settings.Input
{
    [System.Serializable]
    public class DesignAppKeys : KeysCollection
    {
        #region fields & properties
        public KeyCodeInfo MoveUpKey => moveUpKey;
        [SerializeField] private KeyCodeInfo moveUpKey = new(KeyCode.W, KeyCodeDescription.DesignMoveUp);
        public KeyCodeInfo MoveDownKey => moveDownKey;
        [SerializeField] private KeyCodeInfo moveDownKey = new(KeyCode.S, KeyCodeDescription.DesignMoveDown);
        public KeyCodeInfo MoveRightKey => moveRightKey;
        [SerializeField] private KeyCodeInfo moveRightKey = new(KeyCode.D, KeyCodeDescription.DesignMoveRight);
        public KeyCodeInfo MoveLeftKey => moveLeftKey;
        [SerializeField] private KeyCodeInfo moveLeftKey = new(KeyCode.A, KeyCodeDescription.DesignMoveLeft);
        public KeyCodeInfo RotateKey => rotateKey;
        [SerializeField] private KeyCodeInfo rotateKey = new(KeyCode.R, KeyCodeDescription.DesignRotate);
        public KeyCodeInfo DeselectKey => deselectKey;
        [SerializeField] private KeyCodeInfo deselectKey = new(KeyCode.Q, KeyCodeDescription.DesignDeselect);
        public KeyCodeInfo RemoveKey => removeKey;
        [SerializeField] private KeyCodeInfo removeKey = new(KeyCode.G, KeyCodeDescription.DesignRemove);
        public KeyCodeInfo DuplicateKey => duplicateKey;
        [SerializeField] private KeyCodeInfo duplicateKey = new(KeyCode.C, KeyCodeDescription.DesignDuplicate);
        public KeyCodeInfo FocusKey => focusKey;
        [SerializeField] private KeyCodeInfo focusKey = new(KeyCode.F, KeyCodeDescription.DesignFocus);
        #endregion fields & properties

        #region methods
        public override List<KeyCodeInfo> GetKeys()
        {
            List<KeyCodeInfo> list = new()
            {
                MoveUpKey,
                MoveDownKey,
                MoveRightKey,
                MoveLeftKey,
                RotateKey,
                DeselectKey,
                RemoveKey,
                DuplicateKey,
                FocusKey
            };

            return list;
        }
        #endregion methods
    }
}
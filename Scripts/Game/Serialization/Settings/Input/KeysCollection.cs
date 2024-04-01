using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.Settings.Input
{
    [System.Serializable]
    public abstract class KeysCollection
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public abstract List<KeyCodeInfo> GetKeys();
        #endregion methods
    }
}
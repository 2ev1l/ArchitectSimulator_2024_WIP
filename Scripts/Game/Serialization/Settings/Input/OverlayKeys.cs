using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Serialization.Settings.Input
{
    [System.Serializable]
    public class OverlayKeys : KeysCollection
    {
        #region fields & properties
        public DesignAppKeys DesignAppKeys => designAppKeys;
        [SerializeField] private DesignAppKeys designAppKeys = new();
        #endregion fields & properties

        #region methods
        public override List<KeyCodeInfo> GetKeys()
        {
            List<KeyCodeInfo> list = new();
            list.AddRange(designAppKeys.GetKeys());
            return list;
        }
        #endregion methods
    }
}
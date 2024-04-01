using Game.Serialization.Settings.Input;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class UIStateChange : PanelStateChange
    {
        #region fields & properties
        public KeyCodeDescription OpenKey => openKey;
        [SerializeField] private KeyCodeDescription openKey;
        public KeyCodeDescription CloseKey => closeKey;
        [SerializeField] private KeyCodeDescription closeKey;
        #endregion fields & properties

        #region methods
        #endregion methods
    }
}
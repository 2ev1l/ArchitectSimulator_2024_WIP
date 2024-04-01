using UnityEngine;
using Game.UI.Text;

namespace Game.UI.Collections
{
    public class ScreenModeItem : IntItem
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            int textId = (FullScreenMode)param switch
            {
                FullScreenMode.ExclusiveFullScreen => 40,
                FullScreenMode.FullScreenWindow => 41,
                FullScreenMode.Windowed => 42,
                _ => -1
            };
            Text.text = LanguageLoader.GetTextByType(TextType.Menu, textId);
            value = param;
        }
        #endregion methods
    }
}
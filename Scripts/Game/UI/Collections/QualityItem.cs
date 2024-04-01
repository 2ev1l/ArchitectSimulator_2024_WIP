using Game.UI.Text;

namespace Game.UI.Collections
{
    public class QualityItem : IntItem
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            value = param;
            int textId = value switch
            {
                0 => 43,
                1 => 44,
                2 => 45,
                3 => 46,
                4 => 47,
                5 => 48,
                _ => -1
            };
            Text.text = LanguageLoader.GetTextByType(TextType.Menu, textId);
        }
        #endregion methods
    }
}
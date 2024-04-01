using Universal.Serialization;

namespace Game.UI.Collections
{
    public class ResolutionsItem : TextItem<SimpleResolution>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(SimpleResolution param)
        {
            value = param;
            Text.text = $"{param.width}x{param.height}";
        }
        #endregion methods
    }
}
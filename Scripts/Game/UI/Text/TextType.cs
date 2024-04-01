using Game.Serialization.Localization;

namespace Game.UI.Text
{
    #region enum
    public enum TextType
    {
        None,
        Menu,
        Game,
        Task,
        Resource,
        Subtitle,
    }
    #endregion enum

    public static class TextTypeExtension
    {
        #region methods
        public static string GetRawText(this TextType textType, int id)
        {
            if (id < 0) return "";

            LanguageData data = TextData.Instance.LoadedData;
            return (textType) switch
            {
                TextType.None => "",
                TextType.Menu => data.MenuData[id],
                TextType.Game => data.GameData[id],
                TextType.Task => data.TasksData[id],
                TextType.Resource => data.ResourcesData[id],
                TextType.Subtitle => data.SubtitlesData[id],
                _ => throw new System.NotImplementedException($"Text Type {textType}"),
            };
        }
        #endregion methods
    }
}
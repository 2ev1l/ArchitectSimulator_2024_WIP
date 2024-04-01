using UnityEngine;

namespace Game.UI.Text
{
    [System.Serializable]
    public struct LanguageInfo
    {
        #region fields & properties
        [SerializeField][Min(-1)] private int id;
        [SerializeField] private TextType textType;
        /// <summary>
        /// This is exactly what you need
        /// </summary>
        public readonly string Text => LanguageLoader.GetTextByType(textType, id);
        public readonly string RawText => textType.GetRawText(id);
        #endregion fields & properties

        #region methods
        public LanguageInfo(int id, TextType textType)
        {
            this.id = id;
            this.textType = textType;
        }
        #endregion methods
    }
}
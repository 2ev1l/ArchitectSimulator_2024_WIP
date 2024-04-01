using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class SubtitleInfo : DBInfo
    {
        #region fields & properties
        public string Text => textInfo.Text;
        [SerializeField] private LanguageInfo textInfo = new(0, TextType.Subtitle);
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
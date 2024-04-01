using EditorCustom.Attributes;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public abstract class ResourceInfo : DBInfo
    {
        #region fields & properties
        public ResourcePrefab Prefab => prefab;
        [SerializeField] private ResourcePrefab prefab;
        public virtual LanguageInfo NameInfo => nameInfo;
        [SerializeField] private LanguageInfo nameInfo = new(0, TextType.Resource);
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
using EditorCustom.Attributes;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class ConstructionResourceInfo : ResourceInfo
    {
        #region fields & properties
        [Tooltip("If set to false, then 'name info' field will be ignored and will be generated automatically")]
        [SerializeField] private bool overrideName = false;
        public override LanguageInfo NameInfo => overrideName ? base.NameInfo : GetNameInfo();
        public BlueprintResource Blueprint
        {
            get => blueprint;
            internal set => blueprint = value;
        }
        [SerializeField] private BlueprintResource blueprint;
        public ConstructionType ConstructionType => constructionType;
        [Title("Construction")][SerializeField] ConstructionType constructionType = ConstructionType.Wall;
        public ConstructionSubtype ConstructionSubtype => constructionSubtype;
        [SerializeField] private ConstructionSubtype constructionSubtype = ConstructionSubtype.Base;
        public ConstructionLocation ConstructionLocation => constructionLocation;
        [SerializeField] private ConstructionLocation constructionLocation = ConstructionLocation.Inside;

        public BuildingType BuildingType => buildingType;
        [Space][SerializeField][BitMask] BuildingType buildingType = BuildingType.House;
        public BuildingStyle BuildingStyle => buildingStyle;
        [SerializeField][BitMask] BuildingStyle buildingStyle = 0;
        public BuildingFloor BuildingFloor => buildingFloor;
        [SerializeField][BitMask] BuildingFloor buildingFloor = BuildingFloor.F1;
        private static readonly Dictionary<ConstructionType, LanguageInfo> LanguageByConstructionType = new()
        {
            { ConstructionType.Floor, new(5, TextType.Resource) },
            { ConstructionType.Roof, new(15, TextType.Resource) }
        };
        private static readonly Dictionary<ConstructionSubtype, LanguageInfo> LanguageByConstructionSubtype = new()
        {
            { ConstructionSubtype.Base, new(0, TextType.Resource) },
            { ConstructionSubtype.CornerIn, new(6, TextType.Resource) },
            { ConstructionSubtype.CornerOut, new(6, TextType.Resource) },
            { ConstructionSubtype.Door, new(2, TextType.Resource) },
            { ConstructionSubtype.Window, new(1, TextType.Resource) },
            { ConstructionSubtype.Staircase, new(20, TextType.Resource) },
        };
        private static LanguageInfo UnknownLanguageInfo = new(14, TextType.Resource);
        #endregion fields & properties

        #region methods
        private LanguageInfo GetNameInfo()
        {
            if (LanguageByConstructionType.TryGetValue(constructionType, out LanguageInfo languageByConstructionTypeInfo))
                return languageByConstructionTypeInfo;
            if (LanguageByConstructionSubtype.TryGetValue(constructionSubtype, out LanguageInfo languageByConstructionSubtypeInfo))
                return languageByConstructionSubtypeInfo;
            return UnknownLanguageInfo;
        }
        #endregion methods
    }
}
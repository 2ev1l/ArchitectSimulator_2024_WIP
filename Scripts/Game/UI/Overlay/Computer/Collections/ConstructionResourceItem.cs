using Game.DataBase;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Collections
{
    public class ConstructionResourceItem : ResourceItem
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI constructionLocationText;
        [SerializeField] private TextMeshProUGUI buildingTypeText;
        [SerializeField] private TextMeshProUGUI buildingStyleText;
        [SerializeField] private TextMeshProUGUI buildingFloorText;
        private static readonly LanguageInfo unknownBuildingStyleInfo = new(14, TextType.Resource);
        private readonly System.Text.StringBuilder stringBuilder = new();
        #endregion fields & properties

        #region methods
        public override string GetName()
        {
            ConstructionResourceInfo info = (ConstructionResourceInfo)Context;
            string nameDefault = info.NameInfo.Text;
            if (info.ConstructionType != ConstructionType.Wall)
            {
                return $"{nameDefault}{base.GetName()}";
            }
            string nameSubtype = info.ConstructionSubtype.ToLanguage();
            if (nameDefault.Equals(nameSubtype))
            {
                return $"{nameDefault}{base.GetName()}";
            }
            else
            {
                return $"{nameDefault}-{nameSubtype}{base.GetName()}";
            }
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();

            ConstructionResourceInfo info = (ConstructionResourceInfo)Context;
            if (constructionLocationText != null)
            {
                constructionLocationText.text = $"{info.ConstructionLocation.ToLanguage()}";
            }
            stringBuilder.Clear();
            if (buildingTypeText != null)
            {
                info.BuildingType.ForEachFlag(x => stringBuilder.Append($"{x.ToLanguage()}, "));
                buildingTypeText.text = TryFixUnknownText(stringBuilder.ToString());
            }

            stringBuilder.Clear();
            if (buildingStyleText != null)
            {
                info.BuildingStyle.ForEachFlag(x => stringBuilder.Append($"{x.ToLanguage()}, "));
                buildingStyleText.text = TryFixUnknownText(stringBuilder.ToString());
            }

            stringBuilder.Clear();
            if (buildingFloorText != null)
            {
                info.BuildingFloor.ForEachFlag(x => stringBuilder.Append($"{x.ToLanguage()}, "));
                buildingFloorText.text = TryFixUnknownText(stringBuilder.ToString());
            }

        }
        private string TryFixUnknownText(string text)
        {
            if (text.Length > 0)
                text = text.Remove(text.Length - 2, 2);
            else
                text = unknownBuildingStyleInfo.Text;
            return text;
        }
        #endregion methods
    }
}
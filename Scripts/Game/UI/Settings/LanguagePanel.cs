using Universal.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.UI.Collections;
using Game.Serialization.Localization;

namespace Game.UI.Settings
{
    public class LanguagePanel : SettingsPanel<LanguageSettings>
    {
        #region fields & properties
        protected override LanguageSettings Settings
        {
            get => Context.LanguageSettings;
            set => Context.LanguageSettings = value;
        }
        [SerializeField] private StringItemList languageList;
        [SerializeField] private Slider characterSpacingSlider;
        [SerializeField] private Slider lineSpacingSlider;
        [SerializeField] private Slider wordSpacingSlider;
        [SerializeField] private IntItemList fontStyleList;
        #endregion fields & properties

        #region methods
        public override LanguageSettings GetNewSettings()
        {
            string choosedLanguage = ExposeItemList(languageList);
            int lineSpacing = (int)ExposeSlider(lineSpacingSlider);
            int wordSpacing = (int)ExposeSlider(wordSpacingSlider);
            int characterSpacing = (int)ExposeSlider(characterSpacingSlider);
            FontStyles fontStyle = (FontStyles)ExposeItemList(fontStyleList);
            LanguageSettings newSettings = new(choosedLanguage, lineSpacing, wordSpacing, characterSpacing, fontStyle);
            return newSettings;
        }

        public override void UpdateUI()
        {
            List<string> languages = LanguageData.GetLanguageNames();
            languageList.SetItems(languages);
            languageList.ItemList.ShowAt(Settings.ChoosedLanguage);

            characterSpacingSlider.value = Settings.CharacterSpacing;
            lineSpacingSlider.value = Settings.LineSpacing;
            wordSpacingSlider.value = Settings.WordSpacing;

            List<int> fontStyles = new() 
            { 
                (int)FontStyles.Normal,
                (int)FontStyles.Bold,
                (int)FontStyles.Italic,
                (int)(FontStyles.Bold | FontStyles.Italic),
                (int)FontStyles.UpperCase,
                (int)FontStyles.SmallCaps,
                (int)(FontStyles.Bold | FontStyles.SmallCaps) 
            };
            fontStyleList.SetItems(fontStyles);
            fontStyleList.ItemList.ShowAt((int)Settings.FontStyle);
        }
        #endregion methods
    }
}
using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class BlueprintTypesStats : TextStatsContent
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            BlueprintEditor.Instance.OnCurrentDataChanged += UpdateUI;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            BlueprintEditor.Instance.OnCurrentDataChanged -= UpdateUI;
        }
        public override void UpdateUI()
        {
            if (!BlueprintEditor.Instance.CanOpenEditor())
            {
                Text.text = "???";
                return;
            }
            BlueprintData data = BlueprintEditor.Instance.CurrentData;
            Text.text = $"{data.BuildingData.BuildingType.ToLanguage()} - {data.BuildingData.BuildingStyle.ToLanguage()}";
        }
        #endregion methods
    }
}
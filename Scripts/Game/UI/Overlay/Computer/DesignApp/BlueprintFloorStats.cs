using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class BlueprintFloorStats : TextStatsContent
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            BlueprintEditor.Instance.Creator.OnFloorChanged += UpdateUI;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            BlueprintEditor.Instance.Creator.OnFloorChanged -= UpdateUI;
        }
        public override void UpdateUI()
        {
            if (!BlueprintEditor.Instance.CanOpenEditor())
            {
                Text.text = "??";
                return;
            }
            Text.text = BlueprintEditor.Instance.Creator.CurrentBuildingFloor.ToLanguage();
        }
        #endregion methods
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.DesignApp
{
    public class BlueprintNameStats : TextStatsContent
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
            Text.text = BlueprintEditor.Instance.CurrentData.Name;
        }
        #endregion methods
    }
}
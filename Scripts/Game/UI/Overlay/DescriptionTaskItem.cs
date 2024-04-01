using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class DescriptionTaskItem : DescriptionItem<TaskData>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI nameText;
        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            base.UpdateUI();
            nameText.text = Context.Info.NameInfo.Text;
        }
        #endregion methods
    }
}
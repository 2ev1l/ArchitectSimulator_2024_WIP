using Game.DataBase;
using Game.UI.Collections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Overlay.Computer.Collections
{
    public class PremiseItem : ContextActionsItem<PremiseInfo>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image previewImage;
        #endregion fields & properties

        #region methods
        protected virtual string GetName()
        {
            return Context.NameInfo.Text;
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            nameText.text = GetName();
            previewImage.sprite = Context.PreviewSprite;
        }
        #endregion methods
    }
}
using Game.DataBase;
using Game.UI.Collections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Overlay.Computer.Collections
{
    public class RentablePremiseItem : ContextActionsItem<RentablePremise>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI rentPriceText;
        [SerializeField] private TextMeshProUGUI purchasePriceText;
        [SerializeField] private PremiseItem premiseItem;
        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            base.UpdateUI();
            purchasePriceText.text = $"${Context.Price}";
            rentPriceText.text = $"${Context.RentPrice} / m.";
        }
        public override void OnListUpdate(RentablePremise param)
        {
            base.OnListUpdate(param);
            premiseItem.OnListUpdate(param.PremiseInfo);
        }
        #endregion methods
    }
}
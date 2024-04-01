using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Collections
{
    public class OfficeItem : PremiseItem
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI maxEmployeesText;
        [SerializeField] private TextMeshProUGUI distanceText;
        #endregion fields & properties

        #region methods
        protected override void UpdateUI()
        {
            base.UpdateUI();
            OfficeInfo info = (OfficeInfo)Context;
            maxEmployeesText.text = $"x{info.MaximumEmployees}";
            distanceText.text = $"{(info.DistanceScale * 2):F2} km";
        }
        #endregion methods
    }
}
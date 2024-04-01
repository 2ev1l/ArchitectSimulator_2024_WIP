using Game.DataBase;
using Game.Serialization.World;
using Game.UI.Overlay.Computer.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class CompanyWarehouseStats : RentablePremiseItem
    {
        #region fields & properties
        private WarehouseData WarehouseData => GameData.Data.CompanyData.WarehouseData;
        [SerializeField] private TextMeshProUGUI freeSpaceText;
        [SerializeField] private TextMeshProUGUI totalItemsText;
        [SerializeField] private GameObject infoPanel;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInfo();
            WarehouseData.OnInfoChanged += CheckInfo;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            WarehouseData.OnInfoChanged -= CheckInfo;
        }
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            WarehouseData.OnSpaceChanged += UpdateUI;
            //don't subscribe on resource change because this actions anyway invokes space change
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            WarehouseData.OnSpaceChanged -= UpdateUI;
        }
        private void CheckInfo()
        {
            if (WarehouseData.Info == null)
                HidePanel();
            else
            {
                ShowPanel();
                OnListUpdate(WarehouseData.RentableInfo);
            }
        }
        private void HidePanel() => infoPanel.SetActive(false);
        private void ShowPanel() => infoPanel.SetActive(true);
        private void UpdateUI(float _) => UpdateUI();
        protected override void UpdateUI()
        {
            base.UpdateUI();
            freeSpaceText.text = $"{WarehouseData.FreeSpace:F2} m3";
            totalItemsText.text = $"{WarehouseData.Resources.Sum(x => x.Count)}x";
        }
        #endregion methods
    }
}
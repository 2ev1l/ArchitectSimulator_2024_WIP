using Game.Serialization.World;
using Game.UI.Overlay.Computer.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class CompanyOfficeStats : RentablePremiseItem
    {
        #region fields & properties
        private OfficeData OfficeData => GameData.Data.CompanyData.OfficeData;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private GameObject nullPanel;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInfo();
            OfficeData.OnInfoChanged += CheckInfo;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            OfficeData.OnInfoChanged -= CheckInfo;
        }

        private void HidePanel()
        {
            infoPanel.SetActive(false);
            nullPanel.SetActive(true);
        }
        private void ShowPanel()
        {
            infoPanel.SetActive(true);
            nullPanel.SetActive(false);
        }
        private void CheckInfo()
        {
            if (OfficeData.Info == null)
                HidePanel();
            else
            {
                ShowPanel();
                OnListUpdate(OfficeData.RentableInfo);
            }
        }
        #endregion methods
    }
}
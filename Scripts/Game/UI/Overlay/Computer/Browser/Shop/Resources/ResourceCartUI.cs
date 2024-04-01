using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ResourceCartUI : ShopCartUI<ResourceShopItemData>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI availableSpaceText;
        [SerializeField] private TextMeshProUGUI requiredSpaceText;

        private float calculatedSpace = 0;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            GameData.Data.CompanyData.WarehouseData.OnSpaceChanged += UpdateUI;
            GameData.Data.CompanyData.WarehouseData.OnInfoChanged += UpdateUI;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            GameData.Data.CompanyData.WarehouseData.OnSpaceChanged -= UpdateUI;
            GameData.Data.CompanyData.WarehouseData.OnInfoChanged -= UpdateUI;
        }
        private void UpdateUI(float _1) => UpdateUI();
        protected override void UpdateUI()
        {
            base.UpdateUI();
            WarehouseData warehouse = GameData.Data.CompanyData.WarehouseData;
            availableSpaceText.text = $"{warehouse.FreeSpace:F2} m3";
            requiredSpaceText.text = $"{calculatedSpace:F2} m3";
            bool canAddResources = warehouse.CanAddResource(calculatedSpace);
            PurchaseButton.enabled = PurchaseButton.enabled && canAddResources;
            requiredSpaceText.color = canAddResources ? NormalColor : BadColor;
        }
        protected override void ResetItemsStats()
        {
            base.ResetItemsStats();
            calculatedSpace = 0;
        }
        protected override void OnCalculateEachItemStats(CountableItem<ResourceShopItemData> x)
        {
            base.OnCalculateEachItemStats(x);
            calculatedSpace += x.Item.Info.ResourceInfo.Prefab.VolumeM3 * x.Count;
        }
        #endregion methods
    }
}
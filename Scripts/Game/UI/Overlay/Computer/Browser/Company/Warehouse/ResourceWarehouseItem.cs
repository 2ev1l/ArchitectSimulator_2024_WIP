using Game.Events;
using Game.Serialization.World;
using Game.UI.Collections;
using Game.UI.Elements;
using Game.UI.Overlay.Computer.Collections;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class ResourceWarehouseItem : ContextActionsItem<ResourceData>
    {
        #region fields & properties
        public WarehouseData Warehouse => GameData.Data.CompanyData.WarehouseData;

        [SerializeField] private TextMeshProUGUI volumeText;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private TextMeshProUGUI totalVolumeText;

        [SerializeField] private CustomButton removeButton;
        [SerializeField] private TMP_InputField inputFieldCount;

        [SerializeField] private ResourceItem resourceItem;
        private static readonly LanguageInfo removeInfo = new(73, TextType.Game);
        private static readonly LanguageInfo removeConfirmInfo = new(84, TextType.Game);
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            removeButton.OnClicked += OnRemoveButtonClicked;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            removeButton.OnClicked -= OnRemoveButtonClicked;
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            volumeText.text = $"{Context.Info.Prefab.VolumeM3:F2} m3";
            quantityText.text = $"{Context.Count}x";
            totalVolumeText.text = $"{(Context.Info.Prefab.VolumeM3 * Context.Count):F2} m3";
        }
        private bool TryReadInputField(out int count)
        {
            count = System.Convert.ToInt32(inputFieldCount.text);
            if (count <= 0) return false;
            return true;
        }
        private void OnRemoveButtonClicked()
        {
            if (!TryReadInputField(out int count)) return;
            ConfirmRequest confirmRemoveRequest = new(OnRemoveConfirmed, null, $"{removeInfo.Text}", $"{removeConfirmInfo.Text}\n\n{this.resourceItem.NameText} x{Mathf.Clamp(count, 0, Context.Count)}");
            confirmRemoveRequest.Send();
        }
        private void OnRemoveConfirmed()
        {
            if (!TryReadInputField(out int count)) return;
            Warehouse.RemoveResource(Context, count);
            UpdateUI();
        }
        public override void OnListUpdate(ResourceData param)
        {
            base.OnListUpdate(param);
            resourceItem.OnListUpdate(Context.Info);
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ResourceShopCartItem : ResourceShopItem
    {
        #region fields & properties
        [Title("Cart")]
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private TextMeshProUGUI totalPriceText;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            Context.ItemData.OnCountChanged += UpdateQuantityText;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            Context.ItemData.OnCountChanged -= UpdateQuantityText;
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            UpdateQuantityText();
        }
        private void UpdateQuantityText(int _) => UpdateQuantityText();
        private void UpdateQuantityText()
        {
            quantityText.text = $"{Context.ItemData.Count}x";
            totalPriceText.text = $"${Context.ItemData.Count * Context.ItemData.Item.FinalPrice}";
        }
        #endregion methods
    }
}
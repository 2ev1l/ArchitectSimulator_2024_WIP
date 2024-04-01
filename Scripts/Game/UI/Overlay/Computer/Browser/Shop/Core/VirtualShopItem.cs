using Game.Serialization.World;
using Game.UI.Collections;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class VirtualShopItem<T> : ContextActionsItem<VirtualShopItemContext<T>> where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Color defaultPriceColor = Color.black;
        [SerializeField] private Color discountPriceColor = Color.red;
        [SerializeField] private CustomButton removeFromCartButton;
        [SerializeField] private CustomButton addToCartButton;
        [SerializeField] private TMP_InputField countInputField;
        [SerializeField] private bool resetCountOnUpdate = false;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            if (addToCartButton != null)
                addToCartButton.OnClicked += AddToCart;
            if (removeFromCartButton != null)
                removeFromCartButton.OnClicked += RemoveFromCart;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            if (addToCartButton != null)
                addToCartButton.OnClicked -= AddToCart;
            if (removeFromCartButton != null)
                removeFromCartButton.OnClicked -= RemoveFromCart;
        }
        public void RemoveFromCart()
        {
            if (!TryGetInputCount(out int count)) return;
            Context.ShopData.RemoveFromCart(Context.ItemData.Item, count);
        }
        public void AddToCart()
        {
            if (!TryGetInputCount(out int count)) return;
            Context.ShopData.AddToCart(Context.ItemData.Item, count);
        }
        private bool TryGetInputCount(out int count)
        {
            count = 1;
            if (countInputField == null) return true;
            count = System.Convert.ToInt32(countInputField.text);
            if (count <= 0) return false;
            return true;
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            string colorDefault = ColorUtility.ToHtmlStringRGB(defaultPriceColor);
            if (!Context.ItemData.Item.HasDiscount)
            {
                priceText.text = $"<color=#{colorDefault}>${Context.ItemData.Item.StartPrice}</color>";
                return;
            }
            string colorDiscount = ColorUtility.ToHtmlStringRGB(discountPriceColor);
            if (priceText != null)
                priceText.text = $"<s><size=75%><color=#{colorDefault}>${Context.ItemData.Item.StartPrice}</color></size></s> " +
                    $"<color=#{colorDiscount}>${Context.ItemData.Item.FinalPrice}</color>";
            if (resetCountOnUpdate && countInputField != null)
                countInputField.text = "1";
        }
        #endregion methods
    }
}
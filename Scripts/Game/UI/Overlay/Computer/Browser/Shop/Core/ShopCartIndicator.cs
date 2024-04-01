using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public class ShopCartIndicator<T> : MonoBehaviour where T : ShopItemData, ICloneable<T>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private GameObject indicator;
        [SerializeField] private VirtualShopBehaviour<T> shop;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            shop.Data.Cart.OnItemAdded += UpdateUI;
            shop.Data.Cart.OnItemRemoved += UpdateUI;
            UpdateUI();
        }
        private void OnDisable()
        {
            shop.Data.Cart.OnItemAdded -= UpdateUI;
            shop.Data.Cart.OnItemRemoved -= UpdateUI;
        }
        private void UpdateUI(CountableItem<T> _) => UpdateUI();
        protected virtual void UpdateUI()
        {
            int count = shop.Data.Cart.Items.Count;
            counter.text = count.ToString();
            indicator.SetActive(count > 0);
        }
        #endregion methods
    }
}
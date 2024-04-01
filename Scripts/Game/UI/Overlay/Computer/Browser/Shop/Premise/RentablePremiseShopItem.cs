using Game.DataBase;
using Game.Events;
using Game.Serialization.World;
using Game.UI.Elements;
using Game.UI.Overlay.Computer.Collections;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.Behaviour;

namespace Game.UI.Overlay.Computer.Browser.Shop
{
    public abstract class RentablePremiseShopItem : VirtualShopItem<RentablePremiseShopItemData>
    {
        #region fields & properties
        public PremiseInfo PremiseInfo => Context.ItemData.Item.Info.PremiseInfo;
        [SerializeField] private TextMeshProUGUI rentPriceText;
        [SerializeField] private PremiseItem premiseItem;
        [SerializeField] private CustomButton instantBuyButton;
        [SerializeField] private DefaultStateMachine sellStates;
        [SerializeField] private StateChange stateSoldOut;
        [SerializeField] private StateChange stateDefault;
        private ConfirmRequest PurchaseConfirmRequest
        {
            get
            {
                purcahseConfirmRequest = new(delegate { DoInstantBuy(); UpdateUI(); }, null, $"{LanguageLoader.GetTextByType(TextType.Game, 71)}", $"" +
                    $"{PremiseInfo.NameInfo.Text}\n" +
                    $"<size=80%>{LanguageLoader.GetTextByType(TextType.Game, 68)}</size>\n\n" +
                    $"<size=85%>{LanguageLoader.GetTextByType(TextType.Game, 6)}: ${Context.ItemData.Item.FinalPrice}\n" +
                    $"{LanguageLoader.GetTextByType(TextType.Game, 78)}: ${Context.ItemData.Item.Info.RentPrice} / m.</size>");
                return purcahseConfirmRequest;
            }
        }
        private ConfirmRequest purcahseConfirmRequest = null;
        private InfoRequest BadPurchaseRequest
        {
            get
            {
                badPurchaseRequest ??= new(null, $"{LanguageLoader.GetTextByType(TextType.Game, 38)}", $"{LanguageLoader.GetTextByType(TextType.Game, 37)}");
                return badPurchaseRequest;
            }
        }
        private InfoRequest badPurchaseRequest = null;
        #endregion fields & properties

        #region methods
        protected override void OnSubscribe()
        {
            base.OnSubscribe();
            instantBuyButton.OnClicked += OnInstantBuyClicked;
        }
        protected override void OnUnSubscribe()
        {
            base.OnUnSubscribe();
            instantBuyButton.OnClicked -= OnInstantBuyClicked;
        }
        protected virtual bool CanDoInstantBuy() => GameData.Data.PlayerData.Wallet.CanDecreaseValue(Context.ItemData.Item.FinalPrice);
        private void OnInstantBuyClicked()
        {
            if (!CanDoInstantBuy())
            {
                BadPurchaseRequest.Send();
                return;
            }
            PurchaseConfirmRequest.Send();
        }
        private void DoInstantBuy()
        {
            Context.ShopData.AddToCart(Context.ItemData.Item, 1);
            Context.ShopData.PurchaseCart();
        }
        protected override void UpdateUI()
        {
            base.UpdateUI();
            rentPriceText.text = $"${Context.ItemData.Item.Info.RentPrice} / m.";
            if (IsSoldOut()) sellStates.ApplyState(stateSoldOut);
            else sellStates.ApplyState(stateDefault);
        }
        protected abstract bool IsSoldOut();
        public override void OnListUpdate(VirtualShopItemContext<RentablePremiseShopItemData> param)
        {
            base.OnListUpdate(param);
            premiseItem.OnListUpdate(PremiseInfo);
        }
        #endregion methods
    }
}
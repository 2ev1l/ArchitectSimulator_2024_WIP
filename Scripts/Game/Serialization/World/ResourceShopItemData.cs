using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class ResourceShopItemData : BuyableObjectItemData<BuyableResource>, ICloneable<ResourceShopItemData>
    {
        #region fields & properties
        public ResourceType ResourceType => resourceType;
        [SerializeField] private ResourceType resourceType;
        #endregion fields & properties

        #region methods
        protected override BuyableResource GetInfo() => resourceType switch
        {
            World.ResourceType.Construction => DB.Instance.BuyableConstructionResourceInfo.Find(x => x.Data.ResourceInfo.Id == Id).Data,
            _ => throw new System.NotImplementedException($"info for {resourceType}") //todo
        };
        public override void OnPurchase(int count)
        {
            base.OnPurchase(count);
            GameData.Data.CompanyData.WarehouseData.TryAddResource(Id, resourceType, count);
        }
        public ResourceShopItemData Clone() => new(Id, StartPrice, Discount, resourceType);

        public ResourceShopItemData(int id, int startPrice, int discount, ResourceType resourceType) : base(id, startPrice, discount)
        {
            this.resourceType = resourceType;
        }
        #endregion methods
    }
}
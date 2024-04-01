using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer.Browser.Company
{
    public class ConstructionResourceWarehouseItemList : ResourceWarehouseItemList
    {
        #region fields & properties
        [SerializeField] private VirtualFilters<ResourceData, ConstructionResourceInfo> constructionResourceInfoFilters = new(x => (ConstructionResourceInfo)x.Info);
        #endregion fields & properties

        #region methods
        protected override IEnumerable<ResourceData> GetFilteredItems(IEnumerable<ResourceData> currentItems)
        {
            currentItems = constructionResourceInfoFilters.ApplyFilters(currentItems);
            return base.GetFilteredItems(currentItems);
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            constructionResourceInfoFilters.Validate(this);
        }
        #endregion methods
    }
}
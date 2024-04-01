using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer
{
    public class ResourceColorFilter : VirtualDropdownFilter<ResourceColor>, ISmartFilter<ResourceInfo>
    {
        #region fields & properties
        public VirtualFilter VirtualFilter => this;
        private IEnumerable<ResourceColor> appliedColors;
        #endregion fields & properties

        #region methods
        public void UpdateFilterData()
        {
            appliedColors = GetEnabledFilters().Select(x => x.Value);
        }
        public bool FilterItem(ResourceInfo item)
        {
            return item.Prefab.MaterialsInfo.Any(x => appliedColors.Any(y => y == x.Color));
        }
        #endregion methods
    }
}
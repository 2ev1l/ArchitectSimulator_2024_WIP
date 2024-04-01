using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Collections.Generic.Filters;
using Universal.Core;

namespace Game.UI.Overlay.Computer
{
    public class ConstructionResourceBuildingTypeFilter : VirtualDropdownFilter<BuildingType>, ISmartFilter<ConstructionResourceInfo>
    {
        #region fields & properties
        public VirtualFilter VirtualFilter => this;
        private IEnumerable<BuildingType> appliedTypes;
        private readonly List<BuildingType> appliedFlags = new();
        #endregion fields & properties

        #region methods
        public void UpdateFilterData()
        {
            appliedTypes = GetEnabledFilters().Select(x => x.Value);
        }
        public bool FilterItem(ConstructionResourceInfo item)
        {
            item.BuildingType.ToFlagList(appliedFlags);
            return appliedFlags.Any(x => appliedTypes.Any(y => y == x));
        }
        #endregion methods
    }
}
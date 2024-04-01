using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Collections.Generic.Filters;
using Universal.Core;

namespace Game.UI.Overlay.Computer
{
    public class ConstructionResourceBuildingStyleFilter : VirtualDropdownFilter<BuildingStyle>, ISmartFilter<ConstructionResourceInfo>
    {
        #region fields & properties
        public VirtualFilter VirtualFilter => this;
        private IEnumerable<BuildingStyle> appliedStyles;
        private readonly List<BuildingStyle> appliedFlags = new();
        #endregion fields & properties

        #region methods
        public void UpdateFilterData()
        {
            appliedStyles = GetEnabledFilters().Select(x => x.Value);
        }
        public bool FilterItem(ConstructionResourceInfo item)
        {
            item.BuildingStyle.ToFlagList(appliedFlags);
            return appliedFlags.Any(x => appliedStyles.Any(y => y == x));
        }
        #endregion methods
    }
}
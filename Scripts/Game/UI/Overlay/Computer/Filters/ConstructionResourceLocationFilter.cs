using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer
{
    public class ConstructionResourceLocationFilter : VirtualDropdownFilter<ConstructionLocation>, ISmartFilter<ConstructionResourceInfo>
    {
        #region fields & properties
        public VirtualFilter VirtualFilter => this;
        private IEnumerable<ConstructionLocation> appliedLocations;
        #endregion fields & properties

        #region methods
        public void UpdateFilterData()
        {
            appliedLocations = GetEnabledFilters().Select(x => x.Value);
        }
        public bool FilterItem(ConstructionResourceInfo item)
        {
            return appliedLocations.Any(x => x == item.ConstructionLocation);
        }
        #endregion methods
    }
}
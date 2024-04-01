using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Collections.Generic.Filters;

namespace Game.UI.Overlay.Computer
{
    public class ConstructionResourceTypeFilter : VirtualDropdownFilter<ConstructionType>, ISmartFilter<ConstructionResourceInfo>
    {
        #region fields & properties
        public VirtualFilter VirtualFilter => this;
        private IEnumerable<ConstructionType> appliedTypes;
        #endregion fields & properties

        #region methods
        public void UpdateFilterData()
        {
            appliedTypes = GetEnabledFilters().Select(x => x.Value);
        }
        public bool FilterItem(ConstructionResourceInfo item)
        {
            return appliedTypes.Any(x => x == item.ConstructionType);
        }
        #endregion methods
    }
}
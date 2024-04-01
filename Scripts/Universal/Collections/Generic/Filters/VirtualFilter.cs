using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Collections.Generic.Filters
{
    public class VirtualFilter : MonoBehaviour
    {
        #region fields & properties
        public bool CanBeApplied => canBeApplied;
        [SerializeField] private bool canBeApplied = false;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            ChangeFilter(canBeApplied);
        }
        protected virtual void OnDisable()
        {

        }
        protected void ChangeFilter(bool canBeApplied)
        {
            this.canBeApplied = canBeApplied;
        }
        public void EnableFilter() => ChangeFilter(true);
        public void DisableFilter() => ChangeFilter(false);
        #endregion methods
    }
}
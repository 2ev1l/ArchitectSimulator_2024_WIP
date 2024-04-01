using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Events;

namespace Game.UI.Overlay
{
    [System.Serializable]
    public class DescriptionPanelRequest<T> : ExecutableRequest where T : class
    {
        #region fields & properties
        public DescriptionItem<T> Item => item;
        private DescriptionItem<T> item;
        #endregion fields & properties

        #region methods
        public override void Close()
        {

        }

        public DescriptionPanelRequest(DescriptionItem<T> item)
        {
            this.item = item;
        }
        #endregion methods
    }
}
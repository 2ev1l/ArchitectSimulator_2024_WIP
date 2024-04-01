using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Collections
{
    public class DescriptionItem<T> : TextItem<T>
    {
        #region fields & properties
        public TextMeshProUGUI TextDescription => textDescription;
        [SerializeField] private TextMeshProUGUI textDescription;
        #endregion fields & properties

        #region methods
        #endregion methods
    }
}
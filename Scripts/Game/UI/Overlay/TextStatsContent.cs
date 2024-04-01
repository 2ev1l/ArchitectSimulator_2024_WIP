using Game.UI.Overlay;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay
{
    public abstract class TextStatsContent : StatsContent
    {
        #region fields & properties
        protected TextMeshProUGUI Text => text;
        [SerializeField] private TextMeshProUGUI text;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
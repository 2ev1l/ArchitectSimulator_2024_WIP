using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Game.UI.Overlay
{
    public class DateText : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int dateSize = 12;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            SetDate();
            InvokeRepeating(nameof(SetDate), 10, 10);
        }
        private void OnDisable()
        {
            CancelInvoke(nameof(SetDate));
        }
        private void SetDate()
        {
            DateTime currentTime = DateTime.Now.ToLocalTime();
            string monthTime = currentTime.ToString("d");
            string hoursTime = currentTime.ToString("t");
            text.text = $"{hoursTime}\n<size={dateSize}>{monthTime}</size>";
        }
        private void OnValidate()
        {
            SetDate();
        }
        #endregion methods
    }
}
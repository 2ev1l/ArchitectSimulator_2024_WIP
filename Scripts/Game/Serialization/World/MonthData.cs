using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class MonthData
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - currentMonth
        /// </summary>
        public UnityAction<int> OnMonthChanged;
        public int CurrentMonth => currentMonth;
        [SerializeField] private int currentMonth = 1;

        public RangedValue FreeTime => freeTime;
        [SerializeField] private RangedValue freeTime = new(168, new(0, 168));
        #endregion fields & properties

        #region methods
        public void StartNextMonth()
        {
            currentMonth++;
            OnMonthChanged?.Invoke(currentMonth);
        }
        #endregion methods
    }
}
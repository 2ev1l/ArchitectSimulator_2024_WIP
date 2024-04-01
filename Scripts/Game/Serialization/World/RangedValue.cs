using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class RangedValue
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - newValue; <br></br>
        /// <see cref="{T1}"/> - changedAmount;
        /// </summary>
        public UnityAction<int, int> OnValueChanged;
        /// <summary>
        /// <see cref="{T0}"/> - newRange;
        /// </summary>
        public UnityAction<Vector2Int> OnRangeChanged;
        public int Value => value;
        [SerializeField] private int value = 0;
        public Vector2Int Range => range;
        [SerializeField] private Vector2Int range = Vector2Int.up;
        public int MinChangesLimit => value - range.x;
        public int MaxChangesLimit => range.y - value;
        #endregion fields & properties

        #region methods
        public bool CanDecreaseValue(int amount) => amount <= MinChangesLimit && amount > 0;
        public bool CanIncreaseValue(int amount) => amount <= MaxChangesLimit && amount > 0;
        public bool TryDecreaseValue(int amount)
        {
            if (!CanDecreaseValue(amount)) return false;
            SetValue(value - amount);
            return true;
        }
        public bool TryIncreaseValue(int amount)
        {
            if (!CanIncreaseValue(amount)) return false;
            SetValue(value + amount);
            return true;
        }
        private void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, range.x, range.y);
            int amountChanged = newValue - value;
            value = newValue;
            OnValueChanged?.Invoke(value, amountChanged);
        }

        /// <summary>
        /// Automatically clamps value to range
        /// </summary>
        public void SetMaxRange(int max)
        {
            range.y = max;
            range.x = Mathf.Min(range.x, range.y);
            SetValue(value);
        }
        /// <summary>
        /// Automatically clamps value to range
        /// </summary>
        public void SetMinRange(int min)
        {
            range.x = min;
            range.y = Mathf.Max(range.y, range.x);
            SetValue(value);
        }
        /// <summary>
        /// Automatically clamps value to range
        /// </summary>
        /// <param name="newRange">x > y</param>
        public void SetRange(Vector2Int newRange)
        {
            if (newRange.x > newRange.y)
                (newRange.x, newRange.y) = (newRange.y, newRange.x);
            range = newRange;
            SetValue(value);
        }

        public RangedValue(int value, Vector2Int range)
        {
            SetRange(range);
            SetValue(value);
        }
        #endregion methods
    }
}
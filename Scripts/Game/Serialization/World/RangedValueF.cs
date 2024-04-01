using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class RangedValueF
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - newValue; <br></br>
        /// <see cref="{T1}"/> - changedAmount;
        /// </summary>
        public UnityAction<float, float> OnValueChanged;
        /// <summary>
        /// <see cref="{T0}"/> - newRange;
        /// </summary>
        public UnityAction<Vector2> OnRangeChanged;
        public float Value => value;
        [SerializeField] private float value = 0;
        public Vector2 Range => range;
        [SerializeField] private Vector2 range = Vector2Int.up;
        /// <summary>
        /// How much value can change until it reaches minimum
        /// </summary>
        public float MinChangesLimit => value - range.x;
        /// <summary>
        /// How much value can change until it reaches maximum
        /// </summary>
        public float MaxChangesLimit => range.y - value;
        #endregion fields & properties

        #region methods
        public bool CanDecreaseValue(float amount) => amount <= MinChangesLimit && amount > 0;
        public bool CanIncreaseValue(float amount) => amount <= MaxChangesLimit && amount > 0;
        public bool TryDecreaseValue(float amount)
        {
            if (!CanDecreaseValue(amount)) return false;
            SetValue(value - amount);
            return true;
        }
        public bool TryIncreaseValue(float amount)
        {
            if (!CanIncreaseValue(amount)) return false;
            SetValue(value + amount);
            return true;
        }
        private void SetValue(float newValue)
        {
            newValue = Mathf.Clamp(newValue, range.x, range.y);
            float amountChanged = newValue - value;
            value = newValue;
            OnValueChanged?.Invoke(value, amountChanged);
        }

        /// <summary>
        /// Automatically clamps value to range
        /// </summary>
        public void SetMaxRange(float max)
        {
            range.y = max;
            range.x = Mathf.Min(range.x, range.y);
            SetValue(value);
        }
        /// <summary>
        /// Automatically clamps value to range
        /// </summary>
        public void SetMinRange(float min)
        {
            range.x = min;
            range.y = Mathf.Max(range.y, range.x);
            SetValue(value);
        }
        /// <summary>
        /// Automatically clamps value to range
        /// </summary>
        /// <param name="newRange">x > y</param>
        public void SetRange(Vector2 newRange)
        {
            if (newRange.x > newRange.y)
                (newRange.x, newRange.y) = (newRange.y, newRange.x);
            range = newRange;
            SetValue(value);
        }

        public RangedValueF(float value, Vector2 range)
        {
            SetRange(range);
            SetValue(value);
        }
        #endregion methods
    }
}
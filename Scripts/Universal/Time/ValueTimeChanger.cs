using System;
using UnityEngine;
using EditorCustom.Attributes;

namespace Universal.Time
{
    [System.Serializable]
    public class ValueTimeChanger : Timer
    {
        #region fields & properties
        public Action<float> OnValueChange;
        public float Value => value;
        [SerializeField][ReadOnly] float value;
        public static AnimationCurve DefaultCurve => AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve Curve
        {
            get => curve;
            set => curve = value;
        }
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        private float startValue;
        private float finalValue;
        #endregion fields & properties

        #region methods
        protected override void OnTimeChanged(float time, float totalSeconds)
        {
            value = Mathf.Lerp(startValue, finalValue, Curve.Evaluate(time / totalSeconds));
            OnValueChange?.Invoke(value);
        }
        protected override void End()
        {
            value = finalValue;
            base.End();
        }
        /// <summary>
        /// Probably you need <see cref="SetValues"/>, <see cref="SetActions"/> and <see cref="Timer.Restart(float)"/>
        /// </summary>
        public ValueTimeChanger()
        {
            curve = DefaultCurve;
        }
        public void SetValues(float startValue, float finalValue)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
        }
        public void SetActions(Action<float> onValueChange, Action onEnd = null, Func<bool> breakCondition = null, bool invokeEndAtBreak = false)
        {
            this.OnValueChange = onValueChange;
            this.OnChangeEnd = onEnd;
            this.BreakCondition = breakCondition;
        }
        #endregion methods
    }
}
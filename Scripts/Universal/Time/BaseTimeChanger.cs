using System;
using System.Collections;
using UnityEngine;

namespace Universal.Time
{
    [System.Serializable]
    public abstract class BaseTimeChanger<T>
    {
        #region fields & properties
        protected Action<T> OnValueChanged;
        public T OutValue => outValue;
        [SerializeField] protected T outValue;

        protected T StartValue => startValue;
        private T startValue;
        protected T FinalValue => finalValue;
        private T finalValue;
        public ValueTimeChanger VTC => vtc;
        [SerializeField] private ValueTimeChanger vtc = new();
        #endregion fields & properties

        #region methods
        public virtual void SetValues(T startValue, T finalValue)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
        }
        protected virtual void OnEnd()
        {
            LerpValue(1);
            VTC.OnChangeEnd -= OnEnd;
        }
        /// <summary>
        /// Set <see cref="OutValue"/> here. Doesn't need to invoke <see cref="OnValueChanged"/>
        /// </summary>
        /// <param name="lerp"></param>
        protected abstract void LerpValue(float lerp);
        private void SetValue(float lerp)
        {
            LerpValue(lerp);
            OnValueChanged?.Invoke(OutValue);
        }
        public virtual void SetActions(Action<T> onValueChange, Action onEnd = null, Func<bool> breakCondition = null, bool invokeEndAtBreak = false)
        {
            this.OnValueChanged = onValueChange;
            VTC.SetActions(SetValue, onEnd, breakCondition, invokeEndAtBreak);
        }
        public virtual void Stop() => VTC.Stop();
        public virtual void Break() => VTC.Break();
        public virtual void Restart(float secondsToWait) 
        {
            VTC.SetValues(0, 1);
            VTC.OnChangeEnd += OnEnd;
            VTC.Restart(secondsToWait);
        }
        public IEnumerator WaitUntilEnd()
        {
            yield return VTC.WaitUntilEnd();
        }
        #endregion methods
    }
}
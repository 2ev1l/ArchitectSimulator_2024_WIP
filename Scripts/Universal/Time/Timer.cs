using System.Collections;
using UnityEngine;
using EditorCustom.Attributes;
using Universal.Behaviour;
using Universal.Core;

namespace Universal.Time
{
    [System.Serializable]
    public class Timer
    {
        #region fields & properties
        public System.Action OnChangeEnd;
        protected System.Func<bool> BreakCondition = null;

        /// <summary>
        /// Seconds
        /// </summary>
        public float Time => time;
        [SerializeField][ReadOnly] private float time = 0;
        public bool IsEnded => isEnded;
        [SerializeField][ReadOnly] private bool isEnded = false;

        private float secondsToWait;
        protected bool InvokeEndAtBreak = true;
        private static MonoBehaviour Context => SingleGameInstance.Instance;
        private Coroutine changeCoroutine = null;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Simulates break condition
        /// </summary>
        public void Break()
        {
            Stop();
            if (InvokeEndAtBreak)
            {
                End();
            }
            else
            {
                time = secondsToWait;
                isEnded = true;
            }
        }
        /// <summary>
        /// Stops timer at current time
        /// </summary>
        public void Stop()
        {
            if (changeCoroutine == null) return;
#if UNITY_EDITOR
            if (Context == null) return;
#endif //UNITY_EDITOR
            Context.StopCoroutine(changeCoroutine);
        }
        /// <summary>
        /// Starts timer ignoring anything
        /// </summary>
        /// <param name="secondsToWait"></param>
        public void Restart(float secondsToWait)
        {
            Stop();
            Start(secondsToWait);
        }
        private void Start(float secondsToWait)
        {
            this.secondsToWait = secondsToWait;
            isEnded = false;
            time = 0;
            if (Context != null)
                changeCoroutine = Context.StartCoroutine(Change());
        }
        private IEnumerator Change()
        {
            while (time < secondsToWait)
            {
                yield return WaitForTimeUnit();
                if (BreakCondition != null && BreakCondition.Invoke())
                {
                    if (InvokeEndAtBreak) break;
                    time = secondsToWait;
                    isEnded = true;
                    yield break;
                }

                OnTimeChanged(time, secondsToWait);
                time += GetTimeUnit();
            }
            End();
        }
        public virtual float GetTimeUnit() => UnityEngine.Time.deltaTime;
        public virtual IEnumerator WaitForTimeUnit()
        {
            yield return CustomMath.WaitAFrame();
        }
        public IEnumerator WaitUntilEnd()
        {
            while (!IsEnded)
                yield return WaitForTimeUnit();
        }
        protected virtual void OnTimeChanged(float lerp, float time) { }
        protected virtual void End()
        {
            time = secondsToWait;
            isEnded = true;
            OnChangeEnd?.Invoke();
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;
using Universal.Core;

namespace Universal.Time
{
    public class DelayedAction : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent delayedEvent;

        [SerializeField] private TimeDelay timeDelay;
        private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            if (SingleGameInstance.Instance != null)
            {
                SingleGameInstance.Instance.StartCoroutine(Unsubscribe());
            }
        }
        private void Subscribe()
        {
            if (isSubscribed) return;
            timeDelay.OnDelayReady += DoActionImmediate;
            isSubscribed = true;
        }
        private IEnumerator Unsubscribe()
        {
            if (!isSubscribed) yield break;
            while (!timeDelay.CanActivate)
            {
                yield return CustomMath.WaitAFrame();
            }
            timeDelay.OnDelayReady -= DoActionImmediate;
            isSubscribed = false;
        }
        [SerializedMethod]
        public void DoActionAtDelay()
        {
            if (!timeDelay.CanActivate) return;
            Subscribe();
            timeDelay.Activate();
        }
        public void DoActionImmediate()
        {
            delayedEvent.Invoke();
        }

        #endregion methods
    }
}
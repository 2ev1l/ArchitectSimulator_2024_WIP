using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Events
{
    public abstract class ResultChecker : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnGoodResultEvent;
        public UnityAction OnGoodResult;
        public UnityEvent OnBadResultEvent;
        public UnityAction OnBadResult;

        [SerializeField] private bool checkOnEnable = true;
        [SerializeField] private bool checkOnAwake = false;
        public bool LastResult => lastResult;
        [SerializeField][ReadOnly] private bool lastResult = false;
        #endregion fields & properties

        #region methods
        protected virtual void Awake()
        {
            if (checkOnAwake)
                Check();
        }
        protected virtual void OnEnable()
        {
            if (checkOnEnable)
                Check();
        }
        protected virtual void OnDisable()
        {

        }
        public void Check()
        {
            lastResult = GetResult();
            if (lastResult)
            {
                OnGoodResult?.Invoke();
                OnGoodResultEvent?.Invoke();
            }
            else
            {
                OnBadResult?.Invoke();
                OnBadResultEvent?.Invoke();
            }
        }
        public abstract bool GetResult();
        #endregion methods
    }
}
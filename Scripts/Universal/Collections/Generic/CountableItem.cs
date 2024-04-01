using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.Collections.Generic
{
    [System.Serializable]
    public class CountableItem<T>
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - currentCount
        /// </summary>
        public UnityAction<int> OnCountChanged;
        public T Item => item;
        [SerializeField] private T item;
        public int Count
        {
            get => count;
            set => SetCount(value);
        }
        [SerializeField][Min(0)] private int count;
        #endregion fields & properties

        #region methods
        private void SetCount(int value)
        {
            value = Mathf.Max(0, value);
            this.count = value;
            OnCountChanged?.Invoke(count);
        }
        public CountableItem(T item, int count)
        {
            this.item = item;
            this.count = count;
        }
        #endregion methods
    }
}
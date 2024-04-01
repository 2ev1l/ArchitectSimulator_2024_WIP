using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Universal.Collections.Generic
{
    [System.Serializable]
    public class CountableItemList<T>
    {
        #region fields & properties
        /// <summary>
        /// Invokes only if new entry was added to list
        /// </summary>
        public UnityAction<CountableItem<T>> OnItemAdded;
        /// <summary>
        /// Invokes only if list entry was removed.
        /// </summary>
        public UnityAction<CountableItem<T>> OnItemRemoved;
        public IReadOnlyList<CountableItem<T>> Items => items;
        [SerializeField] private List<CountableItem<T>> items = new();
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Increasing count or adds new entry in list
        /// </summary>
        public void AddItem(T item, System.Predicate<CountableItem<T>> compare, int count = 1)
        {
            if (items.Exists(compare, out CountableItem<T> exists))
            {
                exists.Count += count;
                return;
            }
            CountableItem<T> newItem = new(item, count);
            items.Add(newItem);
            OnItemAdded?.Invoke(newItem);
        }
        /// <summary>
        /// Decreasing count or removes exists entry in list
        /// </summary>
        public CountableItem<T> RemoveItem(System.Predicate<CountableItem<T>> compare, ref int count)
        {
            if (!items.Exists(compare, out CountableItem<T> exists)) return null;
            count = Mathf.Min(count, exists.Count);
            exists.Count -= count;
            if (exists.Count == 0)
            {
                items.Remove(exists);
                OnItemRemoved?.Invoke(exists);
            }
            return exists;
        }
        /// <summary>
        /// Action invokes per each item
        /// </summary>
        public void Clear()
        {
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; ++i)
            {
                CountableItem<T> removed = items[0];
                items.RemoveAt(0);
                OnItemRemoved?.Invoke(removed);
            }
        }
        #endregion methods
    }
}
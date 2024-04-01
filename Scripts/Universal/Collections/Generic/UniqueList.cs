using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Universal.Collections.Generic
{
    /// <summary>
    /// Serializable dictionary with more behaviour<br></br>
    /// Very bad idea if you think for use it with structs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class UniqueList<T>
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - item
        /// </summary>
        public UnityAction<T> OnItemAdded;
        /// <summary>
        /// <see cref="{T0}"/> - item
        /// </summary>
        public UnityAction<T> OnItemRemoved;
        public IReadOnlyList<T> Items => items;
        [SerializeField] private List<T> items = new();
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Doesn't invoke any actions
        /// </summary>
        public void ClearAllItems() => items.Clear();
        public T Find(System.Predicate<T> match) => items.Find(match);
        public int FindIndex(System.Predicate<T> match) => items.FindIndex(match);
        public bool Exists(System.Predicate<T> match, out T item)
        {
            int itemsCount = items.Count;
            item = default;
            for (int i = 0; i < itemsCount; ++i)
            {
                if (match.Invoke(items[i]))
                {
                    item = items[i];
                    return true;
                }
            }
            return false;
        }
        public bool TryAddItem(T newItem, System.Predicate<T> containsMatch, out T existItem)
        {
            if (Exists(containsMatch, out existItem)) return false;
            AddItem(newItem);
            return true;
        }
        public bool TryAddItem(T newItem, System.Predicate<T> containsMatch)
        {
            if (Exists(containsMatch, out _)) return false;
            AddItem(newItem);
            return true;
        }
        public bool TryRemoveItem(System.Predicate<T> containsMatch)
        {
            if (!Exists(containsMatch, out T item)) return false;
            RemoveItem(item);
            return true;
        }
        public virtual void RemoveItem(T existItem)
        {
            items.Remove(existItem);
            OnItemRemoved?.Invoke(existItem);
        }
        public virtual void AddItem(T nonExistItem)
        {
            items.Add(nonExistItem);
            OnItemAdded?.Invoke(nonExistItem);
        }
        /// <summary>
        /// Invokes actions per each item
        /// </summary>
        public void RemoveAllItems()
        {
            for (int i = 0; i < items.Count; ++i)
            {
                RemoveItem(items[i]);
                --i;
            }
        }
        public UniqueList() { }
        /// <summary>
        /// You should check manually that "uniqueItems" is really unique
        /// </summary>
        /// <param name="uniqueItems"></param>
        public UniqueList(List<T> uniqueItems)
        {
            this.items = uniqueItems;
        }
        #endregion methods
    }
}
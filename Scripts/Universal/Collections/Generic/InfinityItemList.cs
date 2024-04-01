using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Collections.Generic
{
    /// <summary>
    /// Requires ScrollRect with ContentSizeFilter (and Grid Layout Group optional)
    /// </summary>
    [System.Serializable]
    public class InfinityItemList<T, I> where T : Component, IListUpdater<I>
    {
        #region fields & properties
        [SerializeField] private T prefabRoot;
        [SerializeField] private Transform content;
        [SerializeField] private bool showItems = false;
        public IReadOnlyList<T> Items => items;

        protected List<T> items = new();
        protected List<T> itemsPool = new();
        /// <summary>
        /// Unmarshalled game objects
        /// </summary>
        private List<GameObject> itemsGameObjects = new();
        #endregion fields & properties

        #region methods
        private T GetDefaultObject()
        {
            T obj = GameObject.Instantiate(prefabRoot, content);
            GameObject go = obj.gameObject;
#if UNITY_EDITOR
            if (!showItems)
                go.hideFlags = HideFlags.HideInHierarchy;
#endif //UNITY_EDITOR
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            itemsPool.Add(obj);
            itemsGameObjects.Add(go);
            return obj;
        }
        private void UpdateObject(T obj, I param) => obj.OnListUpdate(param);
        private void HideObject(int id)
        {
            GameObject go = itemsGameObjects[id];
            if (!go.activeSelf) return;
            go.SetActive(false);
        }
        public void UpdateListDefault<X>(IEnumerable<X> dataList, System.Func<X, I> updateMatch)
        {
            int count = 0;
            items.Clear();
            foreach (var el in dataList)
            {
                I newData = updateMatch.Invoke(el);
                AddItemWithUI(count, newData);
                count++;
            }
            int countToRemove = itemsPool.Count - count;
            for (int i = 0; i < countToRemove; ++i)
                HideObject(count + i);
        }
        private void AddItemWithUI(int id, I data)
        {
            T item = null;
            try
            {
                item = itemsPool[id];
            }
            catch
            {
                item = GetDefaultObject();
            }
            items.Add(item);

            GameObject go = itemsGameObjects[id];
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            UpdateObject(item, data);
        }
        public InfinityItemList() { }
        public InfinityItemList(T prefabRoot, Transform content)
        {
            this.prefabRoot = prefabRoot;
            this.content = content;
        }
        #endregion methods
    }
}
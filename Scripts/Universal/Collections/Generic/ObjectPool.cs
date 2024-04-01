using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using EditorCustom.Attributes;
using Universal.Core;

namespace Universal.Collections.Generic
{
    /// <summary>
    /// Not as original Flyweight pattern, but combined with Factory. <br></br>
    /// Doesn't using Linq, so creates as less garbage as possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ObjectPool<T> : ICloneable<ObjectPool<T>> where T : Component, IPoolableObject<T>
    {
        #region fields & properties
        public UnityAction<T> OnObjectInstantiated;
        public T OriginalPrefab => originalPrefab;
        [SerializeField] private T originalPrefab;
        public Transform ParentForSpawn => parentForSpawn;
        [SerializeField] private Transform parentForSpawn = null;
        /// <summary>
        /// Affects only on newly spawned objects
        /// </summary>
        public bool HideObjectsInHierarchy
        {
            get => hideObjectsInHierarchy;
            set => hideObjectsInHierarchy = true;
        }
        [SerializeField] private bool hideObjectsInHierarchy = false;
        public IReadOnlyCollection<T> Objects => objects;
        private readonly HashSet<T> objects = new();
        /// <summary>
        /// O(1)
        /// </summary>
        public int ObjectsCount => objects.Count;
        /// <summary>
        /// O(n)
        /// </summary>
        public int ActiveObjectsCount
        {
            get
            {
                int c = 0;
                foreach (T el in objects)
                {
                    if (!el.IsUsing) continue;
                    ++c;
                }
                return c;
            }
        }
        #endregion fields & properties

        #region methods
        /// <summary>
        /// When you got an object, this object will be marked as Used.
        /// </summary>
        /// <returns></returns>
        public T GetObject()
        {
            T obj = null;
            foreach (T el in objects)
            {
                if (el.IsUsing) continue;
                obj = el;
                break;
            }
            if (obj == null)
            {
                if (originalPrefab == null)
                    Debug.LogError("Original prefab is null.");
                obj = originalPrefab.InstantiateThis(parentForSpawn);
#if UNITY_EDITOR
                //don't waste performance on useless operations in game
                if (hideObjectsInHierarchy)
                    obj.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif //UNITY_EDITOR
                obj.OnDestroyed = RemoveObject;
                objects.Add(obj);
                OnObjectInstantiated?.Invoke(obj);
            }
            else obj.FakeInstantiateAgain(obj);
            obj.IsUsing = true;
            return obj;
        }
        public bool TryFindActiveObject(out T obj)
        {
            foreach (T el in objects)
            {
                if (!el.IsUsing) continue;
                obj = el;
                return true;
            }
            obj = null;
            return false;
        }
        /// <summary>
        /// SetActive(false) for objects that <see cref="T.IsUsing"/> but saves <see cref="T.IsUsing"/> state
        /// </summary>
        public void HideUsingObjects()
        {
            foreach (T el in objects)
            {
                if (!el.IsUsing) continue;
                GameObject elGO = el.gameObject;
                if (!elGO.activeSelf) continue;
                elGO.SetActive(false);
            }
        }
        /// <summary>
        /// SetActive(true) for objects that <see cref="T.IsUsing"/> but currently not visible
        /// </summary>
        public void ShowUsingObjects()
        {
            foreach (T el in objects)
            {
                if (!el.IsUsing) continue;
                GameObject elGO = el.gameObject;
                if (elGO.activeSelf) continue;
                elGO.SetActive(true);
            }
        }
        /// <summary>
        /// Disables objects without UI update
        /// </summary>
        public void FakeDisableObjects()
        {
            foreach (T obj in objects)
            {
                obj.IsUsing = false;
            }
        }
        /// <summary>
        /// After using <see cref="FakeDisableObjects"/> this method updates UI for disabled objects
        /// </summary>
        public void FixFakeDisabledObjects()
        {
            foreach (T obj in objects)
            {
                if (obj.IsUsing) continue;
                obj.DisableObject();
            }
        }
        /// <summary>
        /// Disables objects with UI update
        /// </summary>
        public void DisableObjects()
        {
            foreach (T obj in objects)
            {
                obj.DisableObject();
            }
        }
        private void RemoveObject(T obj)
        {
            obj.OnDestroyed = null;
            objects.Remove(obj);
        }
        /// <summary>
        /// Don't create garbage for get <see cref="Objects"/>
        /// </summary>
        /// <param name="newObjects"></param>
        public void SetObjectsTo(HashSet<T> newObjects) => objects.SetElementsTo(newObjects);

        public ObjectPool() { }
        public ObjectPool(T originalPrefab)
        {
            this.originalPrefab = originalPrefab;
        }
        public ObjectPool(T originalPrefab, Transform parentForSpawn)
        {
            this.originalPrefab = originalPrefab;
            this.parentForSpawn = parentForSpawn;
        }
        public ObjectPool<T> Clone()
        {
            ObjectPool<T> newPool = new()
            {
                originalPrefab = originalPrefab,
                parentForSpawn = parentForSpawn,
                hideObjectsInHierarchy = hideObjectsInHierarchy,
            };
            objects.SetElementsTo(newPool.objects);
            return newPool;
        }
        #endregion methods
    }
}
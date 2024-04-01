using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Collections.Generic;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class ResourceData
    {
        #region fields & properties
        public UnityAction OnCountChanged;
        public UnityAction OnResourceRunOut;
        public int Id => id;
        [SerializeField][Min(0)] private int id = 0;
        /// <summary>
        /// Base count is 1
        /// </summary>
        public int Count => count;
        [SerializeField] private int count = 1;
        public ResourceType ResourceType => resourceType;
        [SerializeField] private ResourceType resourceType;
        /// <summary>
        /// O(1)
        /// </summary>
        public bool IsRunOut => Count == 0;
        public ResourceInfo Info
        {
            get
            {
                info ??= GetInfo();
                return info;
            }
        }
        [System.NonSerialized] private ResourceInfo info = null;
        #endregion fields & properties

        #region methods
        public float GetTotalVolumeM3() => Count * Info.Prefab.VolumeM3;
        private ResourceInfo GetInfo() => resourceType switch
        {
            ResourceType.Construction => DB.Instance.ConstructionResourceInfo[Id].Data, //todo
            _ => throw new System.NotImplementedException($"{resourceType}"),
        };
        public void Add(int count)
        {
            count = Mathf.Max(count, 0);
            this.count += count;
            OnCountChanged?.Invoke();
        }
        /// <summary>
        /// Count will be clamped to value range
        /// </summary>
        /// <param name="colorId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public void Remove(ref int count)
        {
            count = Mathf.Clamp(count, 0, Count);
            this.count -= count;
            OnCountChanged?.Invoke();
            if (Count == 0)
                OnResourceRunOut?.Invoke();
        }
        public ResourceData(int id, ResourceType resourceType)
        {
            this.id = id;
            this.resourceType = resourceType;
        }
        #endregion methods
    }
}
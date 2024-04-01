using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class WarehouseData : RentablePremiseData
    {
        #region fields & properties
        public UnityAction<float> OnSpaceChanged;
        /// <summary>
        /// Invokes only for new entries
        /// </summary>
        public UnityAction<ResourceData> OnResourceAdded;
        /// <summary>
        /// Invokes only if resource is completely exhausted
        /// </summary>
        public UnityAction<ResourceData> OnResourceRemoved;
        public float FreeSpace => ((WarehouseInfo)Info).Space - OccupiedSpace;
        /// <summary>
        /// O(1) instead of Resources.Sum
        /// </summary>
        public float OccupiedSpace
        {
            get
            {
                FixOccupiedSpace();
                return occupiedSpace;
            }
            private set => SetOccupiedSpace(value);
        }
        [System.NonSerialized] private float occupiedSpace = -1;
        public IReadOnlyList<ResourceData> Resources => resources.Items;
        [SerializeField] private UniqueList<ResourceData> resources = new();
        #endregion fields & properties

        #region methods
        private void FixOccupiedSpace()
        {
            if (occupiedSpace >= 0) return;
            occupiedSpace = resources.Items.Sum(x => x.GetTotalVolumeM3());
        }
        protected override RentablePremise GetRentablePremiseInfo() => DB.Instance.RentableWarehouseInfo.Find(x => x.Data.PremiseInfo.Id == Id).Data;
        /// <summary>
        /// Compares only by space
        /// </summary>
        /// <param name="newInfoId"></param>
        /// <returns></returns>
        public override bool CanReplaceInfo(int newInfoId)
        {
            WarehouseInfo newInfo = DB.Instance.WarehouseInfo[newInfoId].Data;
            return (newInfo.Space - OccupiedSpace) >= 0;
        }
        protected override void OnInfoReplaced()
        {
            base.OnInfoReplaced();
        }

        private void SetOccupiedSpace(float value)
        {
            FixOccupiedSpace();
            value = Mathf.Max(value, 0);
            occupiedSpace = value;
            OnSpaceChanged?.Invoke(occupiedSpace);
        }
        /// <summary>
        /// Compares only by space
        /// </summary>
        /// <param name="newInfoId"></param>
        /// <returns></returns>

        /// <summary>
        /// All or nothing
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="resourcesVolume"></param>
        /// <returns></returns>
        public bool CanAddResources(IEnumerable<ResourceData> resources, out float resourcesVolume)
        {
            resourcesVolume = 0;
            foreach (var el in resources)
            {
                resourcesVolume += el.GetTotalVolumeM3();
            }
            return CanAddResource(resourcesVolume);
        }
        /// <summary>
        /// All or nothing
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public bool TryAddResources(IEnumerable<ResourceData> resources)
        {
            if (!CanAddResources(resources, out _)) return false;
            foreach (var el in resources)
            {
                AddResource(el, el.GetTotalVolumeM3());
            }
            return true;
        }
        public bool CanAddResource(float resourceVolume) => resourceVolume <= FreeSpace;
        public bool CanAddResource(ResourceData resource) => CanAddResource(resource.GetTotalVolumeM3());
        public bool TryAddResource(ResourceData resource)
        {
            float resourceVolume = resource.GetTotalVolumeM3();
            if (!CanAddResource(resourceVolume)) return false;
            AddResource(resource, resourceVolume);
            return true;
        }
        /// <summary>
        /// If you are sure that resource volume can fit in space
        /// </summary>
        /// <param name="resource"></param>
        private void AddResource(ResourceData resource, float resourceVolume)
        {
            if (resources.TryAddItem(resource, x => x.Id == resource.Id && x.ResourceType == resource.ResourceType, out ResourceData exists))
            {
                OccupiedSpace += resourceVolume;
                OnResourceAdded?.Invoke(resource);
                return;
            }

            //if resource exist than modify its own data
            exists.Add(resource.Count);
            OccupiedSpace += resourceVolume;
        }

        public bool TryAddResource(int id, ResourceType resourceType, int count)
        {
            ResourceData resource = new(id, resourceType);
            resource.Add(count - 1); //resource creates with 1 count
            return TryAddResource(resource);
        }
        public void RemoveResource(int id, ResourceType resourceType, int count)
        {
            if (!resources.Exists(x => x.Id == id && x.ResourceType == resourceType, out ResourceData exists)) return;
            RemoveResource(exists, count);
        }
        /// <summary>
        /// Optimized <see cref="RemoveResource(int, ResourceType, int)"/>
        /// </summary>
        /// <param name="existsResource"></param>
        /// <param name="colorId"></param>
        /// <param name="count"></param>
        public void RemoveResource(ResourceData existsResource, int count)
        {
            if (count == 0) return;
            existsResource.Remove(ref count);
            OccupiedSpace -= count * existsResource.Info.Prefab.VolumeM3;

            if (!existsResource.IsRunOut) return;
            resources.RemoveItem(existsResource);
            OnResourceRemoved?.Invoke(existsResource);
        }

        public WarehouseData(int id) : base(id) { }
        #endregion methods
    }
}
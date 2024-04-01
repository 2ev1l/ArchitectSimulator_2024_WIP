using Game.DataBase;
using Game.Serialization.World;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;
using Universal.Core;

namespace Game.UI.Overlay.Computer.DesignApp
{
    [RequireComponent(typeof(BlueprintResource))]
    internal class BlueprintResourcePlacer : BlueprintPlacerBase
    {
        #region fields & properties
        public BlueprintResource Element
        {
            get
            {
                if (element == null)
                    TryGetComponent(out element);
                return element;
            }
        }
        [SerializeField] private BlueprintResource element;
        /// <summary>
        /// Optimizes get through <see cref="Element"/>, but you shouldn't use this if component was instantiated at the moment
        /// </summary>
        public ConstructionResourceInfo ResourceInfoUnsafe => element.ResourceInfo;
        /// <exception cref="System.NullReferenceException"></exception>
        public ResourceData ResourceData
        {
            get
            {
                if (resourceData == null || Element.ResourceInfo != resourceData.Info)
                {
                    GameData.Data.CompanyData.WarehouseData.Resources.Exists(x => x.Info == Element.ResourceInfo, out resourceData);
                }
                return resourceData;
            }
        }
        private ResourceData resourceData = null;
        private readonly HashSet<BlueprintResourcePlacer> adjacentInsideResources = new();
        private readonly HashSet<BlueprintResourcePlacer> adjacentOutsideResources = new();
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            _ = Element;
            base.OnEnable();
        }
        private bool IsGoodPlacementAdditional(HashSet<BlueprintPlacerBase> lastCollidedBlueprints, HashSet<BlueprintRoom> lastCollidedRooms)
        {
            adjacentInsideResources.Clear();
            adjacentOutsideResources.Clear();
            int totalInside_Outsides = 0;
            int totalInside_Insides = 0;
            int totalOutside_Outsides = 0;
            int totalOutside_Insides = 0;

            bool hasDoorFromInside = false;
            foreach (BlueprintPlacerBase placer in adjacentInsideBlueprints)
            {
                if (placer is not BlueprintResourcePlacer brp) continue;
                adjacentInsideResources.Add(brp);
                ConstructionResourceInfo resInfo = brp.Element.ResourceInfo;
                ConstructionLocation resLoc = resInfo.ConstructionLocation;
                if (resLoc == ConstructionLocation.Inside)
                {
                    totalInside_Insides++;
                }
                else
                {
                    totalInside_Outsides++;
                }
                ConstructionSubtype resSubtype = resInfo.ConstructionSubtype;
                if (resSubtype == ConstructionSubtype.Door)
                    hasDoorFromInside = true;
            }
            if (adjacentInsideResources.Count < 1) return false;
            bool hasDoorFromOutside = false;

            foreach (BlueprintPlacerBase placer in adjacentOutsideBlueprints)
            {
                if (placer is not BlueprintResourcePlacer brp) continue;
                adjacentOutsideResources.Add(brp);
                ConstructionResourceInfo resInfo = brp.Element.ResourceInfo;
                ConstructionLocation resLoc = resInfo.ConstructionLocation;
                if (resLoc == ConstructionLocation.Inside)
                {
                    totalOutside_Insides++;
                }
                else
                {
                    totalOutside_Outsides++;
                }
                ConstructionSubtype resSubtype = resInfo.ConstructionSubtype;
                if (resSubtype == ConstructionSubtype.Door)
                    hasDoorFromOutside = true;
            }

            //disable door lock from side
            if (hasDoorFromInside != hasDoorFromOutside) return false;

            BlueprintResource ownResource = Element;
            ConstructionResourceInfo ownResourceInfo = ownResource.ResourceInfo;
            ConstructionType ownType = ownResourceInfo.ConstructionType;
            ConstructionSubtype ownSubType = ownResourceInfo.ConstructionSubtype;
            if (ownType == ConstructionType.Wall)
            {
                if (ownSubType != ConstructionSubtype.Staircase)
                    if (adjacentOutsideResources.Count < 1) return false;
            }


            ConstructionLocation ownLocation = ownResourceInfo.ConstructionLocation;
            int sameLocationResources = 0;
            if (ownLocation == ConstructionLocation.Inside)
            {
                if (ownSubType != ConstructionSubtype.Staircase)
                    if (totalInside_Insides < 1) return false;
                if (ownType == ConstructionType.Wall)
                {
                    if (ownSubType != ConstructionSubtype.Staircase)
                        if (totalOutside_Insides < 1) return false;
                }
                if (totalOutside_Outsides > 0) return false;
                if (ownSubType == ConstructionSubtype.Door)
                {
                    if (totalOutside_Insides < 2) return false;
                    if (totalInside_Insides < 2) return false;
                }

                //can't connect for non 'base wall' or 'outer corner' with outside resources
                foreach (BlueprintResourcePlacer resource in adjacentInsideResources)
                {
                    ConstructionResourceInfo resInfo = resource.Element.ResourceInfo;
                    if (resInfo.ConstructionLocation != ConstructionLocation.Outside) continue;
                    if (resInfo.ConstructionType != ConstructionType.Wall) return false;
                    if (resInfo.ConstructionSubtype == ConstructionSubtype.Base) continue;
                    if (resInfo.ConstructionSubtype == ConstructionSubtype.CornerOut) continue;
                    return false;
                }
                //can't connect outside resource to own outside part
                foreach (BlueprintResourcePlacer resource in adjacentOutsideResources)
                {
                    ConstructionResourceInfo resInfo = resource.Element.ResourceInfo;
                    if (resInfo.ConstructionLocation != ConstructionLocation.Outside) continue;
                    return false;
                }
                sameLocationResources = totalInside_Insides;
            }
            Vector2 ownLocalCenter = ownResource.LocalCenter;

            if (ownLocation == ConstructionLocation.Outside)
            {
                if (totalInside_Outsides < 2) return false;
                if (totalOutside_Outsides < 2) return false;
                //can't connect without corners
                foreach (BlueprintResourcePlacer resource in adjacentOutsideResources)
                {
                    BlueprintResource res = resource.Element;
                    ConstructionResourceInfo resInfo = res.ResourceInfo;
                    if (resInfo.ConstructionType != ownType || resInfo.ConstructionSubtype != ownSubType || resInfo.ConstructionLocation != ownLocation) continue;
                    Vector2 resLocalCenter = res.LocalCenter;
                    if (resLocalCenter.x.Approximately(ownLocalCenter.x, BlueprintEditor.VECTOR_WORKFLOW_PRECISION) || resLocalCenter.y.Approximately(ownLocalCenter.y, BlueprintEditor.VECTOR_WORKFLOW_PRECISION))
                    {
                        //if (res.RotationScale % 2 != ownResource.RotationScale % 2) return false;
                        if (!adjacentInsideResources.Contains(resource)) return false;
                        continue;
                    }
                    return false;
                }
                sameLocationResources = totalInside_Outsides;
            }
            if (sameLocationResources < 2)
            {
                if (ownSubType == ConstructionSubtype.CornerIn || ownSubType == ConstructionSubtype.CornerOut) return false;
            }

            //can't connect without corners
            foreach (BlueprintResourcePlacer resource in adjacentInsideResources)
            {
                BlueprintResource res = resource.Element;
                ConstructionResourceInfo resInfo = res.ResourceInfo;
                if (resInfo.ConstructionType != ConstructionType.Wall) continue;
                if (resInfo.ConstructionSubtype != ownSubType || resInfo.ConstructionLocation != ownLocation) continue;
                Vector2 resLocalCenter = res.LocalCenter;
                if (resLocalCenter.x.Approximately(ownLocalCenter.x, BlueprintEditor.VECTOR_WORKFLOW_PRECISION) || resLocalCenter.y.Approximately(ownLocalCenter.y, BlueprintEditor.VECTOR_WORKFLOW_PRECISION))
                {
                    if (res.RotationScale % 2 != ownResource.RotationScale % 2) return false;
                    if (!adjacentOutsideResources.Contains(resource)) return false;
                    continue;
                }
                return false;
            }

            return true;
        }
        protected override void CheckFastPlacement(out HashSet<BlueprintPlacerBase> lastCollidedBlueprints, out HashSet<BlueprintRoom> lastCollidedRooms)
        {
            base.CheckFastPlacement(out lastCollidedBlueprints, out lastCollidedRooms);
            if (!IsGoodPlacement) return;
            IsGoodPlacement = IsGoodPlacementAdditional(lastCollidedBlueprints, lastCollidedRooms);
            if (IsGoodPlacement) return; //don't invoke ui twice
            UpdateUI();
        }
        public override void RemoveBlueprint()
        {
            BlueprintEditor.Instance.Creator.CurrentFloor.RemoveResource(this);
            BlueprintPlacerBase obj = null;
            if (BlueprintEditor.Instance.Creator.CurrentFloor.ResourcesPool.TryGetValue(Element.ConstructionReferenceId, out ObjectPool<BlueprintPlacerBase> objectPool))
            {
                objectPool.TryFindActiveObject(out obj);
            }
            BlueprintEditor.Instance.Selector.TrySelectElement(obj);
        }
        public override void CloneBlueprint()
        {
            BlueprintResourcePlacer placer = BlueprintEditor.Instance.Creator.CurrentFloor.SpawnResource(Element.ResourceInfo.Id, Element.Transform.localPosition, Element.RotationScale, Element.ChoosedColor, false);
            BlueprintEditor.Instance.Selector.TrySelectElement(placer);
            placer.CheckDeepPlacementSmoothly();
        }
        #endregion methods
    }
}
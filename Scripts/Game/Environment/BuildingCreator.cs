using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Serialization.World;
using Game.DataBase;

namespace Game.Environment
{
    public class BuildingCreator : MonoBehaviour
    {
        #region fields & properties
        private const float GROUND_OFFSET = 0.2f;
        private const float WORKFLOW_TO_WORLD_SCALE = 200;

        private Transform ParentForSpawn
        {
            get
            {
                if (parentForSpawn == null)
                    parentForSpawn = transform;
                return parentForSpawn;
            }
        }
        private Transform parentForSpawn;
        [SerializeField] private BuildingReference reference;
        private ConstructionData data;
        private bool isConstructionBuild = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            LoadData();
            TryBuildConstruction();
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }
        private void Subscribe()
        {
            if (data == null) return;
            data.OnBuildCompleted += TryBuildConstruction;
        }
        private void Unsubscribe()
        {
            if (data == null) return;
            data.OnBuildCompleted -= TryBuildConstruction;
        }
        private void LoadData()
        {
            GameData.Data.ConstructionsData.TryGet(reference, out data);
        }
        private void TryBuildConstruction()
        {
            if (isConstructionBuild) return;
            if (data == null) return;
            if (!data.IsBuilded)
            {
                //todo place somewhat when not builded and return
            }
            BuildConstruction();
            isConstructionBuild = true;
        }
        private void BuildConstruction()
        {
            BuildingFloor currentFloor = BuildingFloor.F1_Flooring;
            BuildingFloor prevFloor = currentFloor;
            BuildingFloor maxFloor = data.BuildingData.MaxFloor;
            int currentF2Count = 0;
            float currentOffsetY = GROUND_OFFSET;

            while (prevFloor != maxFloor)
            {
                SpawnResources(currentOffsetY, currentFloor, out float increaseOffsetY);
                if (currentFloor == BuildingFloor.F2_FlooringRoof)
                {
                    //todo check for f2 count
                }
                prevFloor = currentFloor;
                currentFloor = currentFloor.GetNextFloor();
                currentOffsetY += increaseOffsetY;
                if (currentFloor == 0) break;
            }
        }
        private void SpawnResources(float currentOffsetY, BuildingFloor currentFloor, out float increaseOffsetY)
        {
            bool increaseInitialized = false;
            increaseOffsetY = 0;
            foreach (BlueprintResourceData resource in data.BlueprintResources)
            {
                if (resource.UnitData.FloorPlaced != currentFloor) continue;
                ConstructionResourceInfo resourceInfo = resource.ResourceInfo;
                if (!increaseInitialized)
                {
                    increaseOffsetY = resourceInfo.Prefab.SizeMeters.y;
                    increaseInitialized = true;
                }
                SpawnResource(currentOffsetY, resource, resourceInfo);
            }
        }
        private void SpawnResource(float offsetY, BlueprintResourceData resource, ConstructionResourceInfo resourceInfo)
        {
            ResourcePrefab instantiated = Instantiate(resourceInfo.Prefab, ParentForSpawn);
            Transform instantiatedTransform = instantiated.Transform;
            Vector3 workflowPosition = resource.UnitData.LocalPosition;
            instantiatedTransform.localPosition = new(workflowPosition.x / WORKFLOW_TO_WORLD_SCALE, offsetY, workflowPosition.y / WORKFLOW_TO_WORLD_SCALE);
            Vector3 ownEulerAngles = instantiatedTransform.localEulerAngles;
            int additionalAngleScale = resource.UnitData.Rotation % 2 == 0 ? 180 : 0;
            Vector3 newLocalEulerAngles = new(ownEulerAngles.x, ownEulerAngles.y + additionalAngleScale + (int)(90 * resource.UnitData.Rotation), ownEulerAngles.z);
            Debug.Log($"{workflowPosition} vs {instantiatedTransform.localPosition}", instantiated);
            //Debug.Log($"{resource.UnitData.Rotation} vs {newLocalEulerAngles}", instantiated);
            instantiatedTransform.localEulerAngles = newLocalEulerAngles;
            instantiated.ChangeColor(resource.ChoosedColorId);
        }
        #endregion methods
    }
}
using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Universal.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Game.DataBase
{
    public class BlueprintResource : BlueprintGraphic
    {
        #region fields & properties
        public int ConstructionReferenceId => constructionReferenceId;
        [SerializeField][Min(-1)] private int constructionReferenceId = 0;
        public ConstructionResourceInfo ResourceInfo
        {
            get
            {
                if (resourceInfo == null || resourceInfo.Id != ConstructionReferenceId)
                {
                    resourceInfo = DB.Instance.ConstructionResourceInfo[ConstructionReferenceId].Data;
                }
                return resourceInfo;
            }
        }
        private ConstructionResourceInfo resourceInfo = null;

        /// <summary>
        /// Will be overwritten when <see cref="ConstructionReferenceId"/> is changed
        /// </summary>
        public int ChoosedColor => choosedColor;
        private int choosedColor = 0;
        [SerializeField] private Image colorImage;
        private BlueprintGraphicUnitState[] ChildGraphicStates
        {
            get
            {
                childGraphicStates ??= GetComponentsInChildren<BlueprintGraphicUnitState>(true);
                return childGraphicStates;
            }
        }
        [System.NonSerialized] private BlueprintGraphicUnitState[] childGraphicStates = null;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Color also will be overwritten
        /// </summary>
        public void ChangeReference(int newReferenceId)
        {
            constructionReferenceId = newReferenceId;
            _ = ResourceInfo;
            ChangeResourceColor(0);
        }
        public void ChangeResourceColor(int newColorId)
        {
            choosedColor = newColorId;
            if (colorImage != null)
                colorImage.color = ResourceInfo.Prefab.MaterialsInfo[choosedColor].Color.ToColorRGB();
        }
        public override void ReplaceWithMultipleInstantiating(BlueprintGraphic newElement)
        {
            ChangeReference(((BlueprintResource)newElement).ConstructionReferenceId);
            ReplaceChilds(ResourceInfo);
            base.ReplaceWithMultipleInstantiating(newElement);
        }
        private void ReplaceChilds(ConstructionResourceInfo info)
        {
            bool isAnyActive = false;
            int childsCount = ChildGraphicStates.Length;
            for (int i = 0; i < childsCount; ++i)
            {
                if (!childGraphicStates[i].TrySetActive(info.ConstructionType, info.ConstructionSubtype, info.ConstructionLocation)) continue;
                isAnyActive = true;
            }

            childGraphicStates[0].SetActive(!isAnyActive);
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!enabled) return;
            if (ChildBase == null)
            {
                if (transform.GetChild(0).TryGetComponent(out RectTransform rt))
                    ChildBase = rt;
            }
            if (BaseCollider == null)
            {
                if (ChildBase.TryGetComponent(out BoxCollider2D bc))
                    BaseCollider = bc;
            }
            if (ChildGraphics == null || ChildGraphics.Length == 0)
            {
                ChildGraphics = GetComponentsInChildren<Graphic>();
            }
            if (RectCollisionCheck == null)
            {
                RectCollisionCheck = ChildBase;
            }
            if (OutsideLines.ExistsEquals((x, y) => x == y && x != null))
            {
                Debug.LogError("Outside lines are not unique");
            }
            if (InsideLines.ExistsEquals((x, y) => x == y && x != null))
            {
                Debug.LogError("Inside lines are not unique");
            }
        }
        [Button(nameof(GetAllGraphics))]
        private void GetAllGraphics()
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(gameObject, "Get child graphics");
            List<Graphic> graphics = new()
            {
                transform.GetChild(0).GetComponent<Graphic>()
            };
            graphics.AddRange(transform.GetComponentsInChildren<Graphic>(true).Where(x => x.transform.parent != transform));
            ChildGraphics = graphics.ToArray();
            RectLine[] rectLines = transform.GetComponentsInChildren<RectLine>(true);
            InsideLines = rectLines.Where(x => x.LineLocation == ConstructionLocation.Inside).ToArray();
            OutsideLines = rectLines.Where(x => x.LineLocation == ConstructionLocation.Outside).ToArray();
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
        [Button(nameof(ActivateAndDeleteChilds))]
        private void ActivateAndDeleteChilds()
        {
            if (resourceReference == null) return;
            UnityEditor.Undo.RegisterCompleteObjectUndo(gameObject, "Set active childs");
            var childs = GetComponentsInChildren<BlueprintGraphicUnitState>(true);
            bool isAnyActive = false;
            for (int i = 1; i < childs.Length; ++i)
            {
                if (!childs[i].TrySetActive(resourceReference.Data.ConstructionType, resourceReference.Data.ConstructionSubtype, resourceReference.Data.ConstructionLocation))
                {
                    DestroyImmediate(childs[i].GameObject);
                }
                else
                {
                    isAnyActive = true;
                }
            }
            if (isAnyActive)
            {
                DestroyImmediate(childs[0].GameObject);
            }
            else
            {
                childs[0].SetActive(true);
            }
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
        [Title("Creator")]
        [SerializeField] private ConstructionResourceInfoSO resourceReference;

        [Button(nameof(CreateAndConnect))]
        private void CreateAndConnect()
        {
            if (resourceReference == null) return;
            UnityEditor.Undo.RegisterCompleteObjectUndo(gameObject, "Create Blueprint Element");
            constructionReferenceId = resourceReference.Id;
            Vector3Int sizeCm = resourceReference.Data.Prefab.SizeCentimeters;
            float referenceLengthCm = sizeCm.x;
            float referenceWidthCm = sizeCm.z;
            float rectWidth = referenceLengthCm * CentimetersToRectScale;
            float rectHeight = referenceWidthCm * CentimetersToRectScale;
            base.ChangeSize(rectWidth, rectHeight);
            ResetRotation();
            ActivateAndDeleteChilds();
            UnityEditor.EditorUtility.SetDirty(gameObject);

            string newName = $"Blueprint Element {resourceReference.Id}";
            gameObject.name = newName;
            string assetPath = $"Assets/Prefabs/UI/Game Overlay/Design App/Blueprints/{newName}.prefab";
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
            GameObject createdAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, assetPath, InteractionMode.UserAction);

            UnityEditor.Undo.RegisterCompleteObjectUndo(resourceReference, "Create Blueprint Element");
            resourceReference.Data.Blueprint = createdAsset.GetComponent<BlueprintResource>();
            UnityEditor.EditorUtility.SetDirty(resourceReference);
        }
        [Button(nameof(RecreateForAllAssets))]
        private void RecreateForAllAssets()
        {
            if (!EditorUtility.DisplayDialog("Recreate resources for all assets", "All blueprint prefabs will be rewritten. Are you sure you want to do this?", "Yes", "No")) return;
            foreach (ConstructionResourceInfoSO resource in DB.Instance.ConstructionResourceInfo.Data)
            {
                resourceReference = resource;
                CreateAndConnect();
            }
        }
#endif //UNITY_EDITOR

    }
}
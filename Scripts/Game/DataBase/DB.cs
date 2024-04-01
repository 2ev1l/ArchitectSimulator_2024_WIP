using UnityEngine;
using EditorCustom.Attributes;
using Universal.Core;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Game.DataBase
{
    [ExecuteAlways]
    public class DB : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public static DB Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.FindFirstObjectByType<DB>();
                return instance;
            }
            private set => instance = value;
        }
        private static DB instance;
        public DBSOSet<MoodInfoSO> MoodInfo => moodInfo;
        [SerializeField] private DBSOSet<MoodInfoSO> moodInfo;
        public DBSOSet<TaskInfoSO> TaskInfo => taskInfo;
        [SerializeField] private DBSOSet<TaskInfoSO> taskInfo;

        public DBSOSet<ConstructionResourceInfoSO> ConstructionResourceInfo => constructionResourceInfo;
        [SerializeField] private DBSOSet<ConstructionResourceInfoSO> constructionResourceInfo;
        /// <summary>
        /// Don't use <see cref="DBSOSet{TypeSO}.GetObjectById(int)"/>
        /// </summary>
        public DBSOSet<BuyableConstructionResourceSO> BuyableConstructionResourceInfo => buyableConstructionResourceInfo;
        [SerializeField] private DBSOSet<BuyableConstructionResourceSO> buyableConstructionResourceInfo;

        public DBSOSet<WarehouseInfoSO> WarehouseInfo => warehouseInfo;
        [SerializeField] private DBSOSet<WarehouseInfoSO> warehouseInfo;
        /// <summary>
        /// Don't use <see cref="DBSOSet{TypeSO}.GetObjectById(int)"/>
        /// </summary>
        public DBSOSet<RentableWarehouseSO> RentableWarehouseInfo => rentableWarehouseInfo;
        [SerializeField] private DBSOSet<RentableWarehouseSO> rentableWarehouseInfo;

        public DBSOSet<OfficeInfoSO> OfficeInfo => officeInfo;
        [SerializeField] private DBSOSet<OfficeInfoSO> officeInfo;
        /// <summary>
        /// Don't use <see cref="DBSOSet{TypeSO}.GetObjectById(int)"/>
        /// </summary>
        public DBSOSet<RentableOfficeSO> RentableOfficeInfo => rentableOfficeInfo;
        [SerializeField] private DBSOSet<RentableOfficeSO> rentableOfficeInfo;

        public DBSOSet<SubtitleInfoSO> SubtitleInfo => subtitleInfo;
        [SerializeField] private DBSOSet<SubtitleInfoSO> subtitleInfo;
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        #endregion methods

#if UNITY_EDITOR
        [SerializeField] private bool automaticallyUpdateEditor = true;
        private void OnValidate()
        {
            if (!automaticallyUpdateEditor) return;
            GetAllDBInfo();
            CheckAllErrors();
        }
        /// <summary>
        /// You need to manually add code
        /// </summary>
        [Button(nameof(GetAllDBInfo))]
        private void GetAllDBInfo()
        {
            if (Application.isPlaying) return;
            AssetDatabase.Refresh();
            Undo.RegisterCompleteObjectUndo(this, "Update DB");

            //call dbset.CollectAll()
            moodInfo.CollectAll();
            taskInfo.CollectAll();
            constructionResourceInfo.CollectAll();
            buyableConstructionResourceInfo.CollectAll();
            warehouseInfo.CollectAll();
            rentableWarehouseInfo.CollectAll();
            officeInfo.CollectAll();
            rentableOfficeInfo.CollectAll();
            subtitleInfo.CollectAll();

            EditorUtility.SetDirty(this);
        }
        /// <summary>
        /// You need to manually add code
        /// </summary>
        [Button(nameof(CheckAllErrors))]
        private void CheckAllErrors()
        {
            if (!Application.isPlaying) return;
            //call dbset.CatchExceptions(x => ...)
            System.Exception e = new();

            moodInfo.CatchExceptions(x => _ = x.Data.Sprite == null ? throw e : 0, "Sprite is null");

            taskInfo.CatchExceptions(x => _ = x.Data.NameInfo.Text, "Wrong Text Name");
            taskInfo.CatchExceptions(x => _ = x.Data.DescriptionInfo.Text, "Wrong Text Description");
            taskInfo.CatchExceptions(x => x.Data.RewardInfo.Rewards.Exists(x => x.Value == 0 && x.Type == RewardType.Rating, out _), e, "Check rewards value in rating type");
            taskInfo.CatchExceptions(x => x.Data.RewardInfo.Rewards.Exists(x => x.Value == 0 && x.Type == RewardType.Money, out _), e, "Check rewards value in money type");
            taskInfo.CatchExceptions(x => x.Data.RewardInfo.Rewards.Exists(x => x.Value == 0 && x.Type == RewardType.Mood, out _), e, "Check rewards value in mood type");
            taskInfo.CatchExceptions(x => x.Data.RewardInfo.Rewards.ExistsEquals((x, y) => x.Value == y.Value && x.Type == y.Type), e, "Rewards must be unique");
            taskInfo.CatchExceptions(x => x.Data.NextTasksTrigger.ExistsEquals((x, y) => x == y), e, "Next tasks must be unique");
            taskInfo.CatchExceptions(x => x.Data.StartSubtitlesTrigger.ExistsEquals((x, y) => x == y), e, "Subtitles must be unique");

            CatchResourceReferenceExceptions<ConstructionResourceInfoSO, ConstructionResourceInfo>(constructionResourceInfo);
            constructionResourceInfo.CatchExceptions(x => x.Data.Blueprint == null, e, "Check blueprint");
            constructionResourceInfo.CatchExceptions(x => x.Data.ConstructionType == ConstructionType.Wall &&
                    (x.Data.BuildingFloor.HasFlag(BuildingFloor.F2_FlooringRoof) ||
                    x.Data.BuildingFloor.HasFlag(BuildingFloor.F1_Flooring) ||
                    x.Data.BuildingFloor.HasFlag(BuildingFloor.F3_Roof)), e, "Check floor flags");
            constructionResourceInfo.CatchExceptions(x => x.Data.ConstructionType == ConstructionType.Floor &&
                    (x.Data.BuildingFloor.HasFlag(BuildingFloor.F1) ||
                    x.Data.BuildingFloor.HasFlag(BuildingFloor.F2) ||
                    x.Data.BuildingFloor.HasFlag(BuildingFloor.F3_Roof)), e, "Check floor flags");
            constructionResourceInfo.CatchExceptions(x => x.Data.ConstructionType == ConstructionType.Roof &&
                    (x.Data.BuildingFloor.HasFlag(BuildingFloor.F1) ||
                    x.Data.BuildingFloor.HasFlag(BuildingFloor.F2) ||
                    x.Data.BuildingFloor.HasFlag(BuildingFloor.F1_Flooring)), e, "Check floor flags");

            constructionResourceInfo.CatchExceptions(x => x.Data.ConstructionType == ConstructionType.Floor &&
                    x.Data.ConstructionSubtype != ConstructionSubtype.Base, e, "Check subtype");

            constructionResourceInfo.CatchExceptions(x => x.Data.ConstructionType == ConstructionType.Roof &&
                    x.Data.ConstructionSubtype != ConstructionSubtype.Base, e, "Check subtype");

            constructionResourceInfo.CatchExceptions(x => x.Data.ConstructionSubtype == ConstructionSubtype.Staircase &&
                    x.Data.BuildingFloor.HasFlag(BuildingFloor.F2), e, "Check floor flags");

            CatchPremiseReferenceExceptions<WarehouseInfoSO, WarehouseInfo>(warehouseInfo);
            CatchPremiseReferenceExceptions<OfficeInfoSO, OfficeInfo>(officeInfo);

            CatchBuyableReferenceExceptions<BuyableConstructionResourceSO, BuyableConstructionResource>(buyableConstructionResourceInfo);
            CatchBuyableReferenceExceptions<RentableWarehouseSO, RentableWarehouse>(rentableWarehouseInfo);
            CatchBuyableReferenceExceptions<RentableOfficeSO, RentableOffice>(rentableOfficeInfo);

            warehouseInfo.CatchExceptions(x => x.Data.Space < 0, e, "Space must be >= 0");

            officeInfo.CatchExceptions(x => x.Data.MaximumEmployees <= 0, e, "Max employees must be > 0");
            officeInfo.CatchExceptions(x => x.Data.DistanceScale < 0, e, "Distance scale must be >= 0");

            subtitleInfo.CatchDefaultExceptions();
        }
        private void CatchPremiseReferenceExceptions<SO, Data>(DBSOSet<SO> dbset) where SO : PremiseInfoSO<Data> where Data : PremiseInfo
        {
            System.Exception e = new();
            dbset.CatchExceptions(x => x.Data.PreviewSprite == null, e, "Sprite is null");
        }
        private void CatchResourceReferenceExceptions<SO, Data>(DBSOSet<SO> dbset) where SO : ResourceInfoSO<Data> where Data : ResourceInfo
        {
            System.Exception e = new();
            dbset.CatchExceptions(x => _ = x.Data.NameInfo.Text, "Wrong Text Name");
            dbset.CatchExceptions(x => _ = x.Data.Prefab == null, e, "Prefab is null");
            dbset.CatchExceptions(x => x.Data.Prefab.MaterialsInfo.Count == 0, e, "Materials count is zero");
            dbset.CatchExceptions(x => x.Data.Prefab.MaterialsInfo.Exists(x => x.Materials.Count == 0, out _), e, "Materials must be > 0");
            dbset.CatchExceptions(x => x.Data.Prefab.MaterialsInfo.ExistsEquals((x, y) => x.Color == y.Color), e, "Colors is not unique");
            dbset.CatchExceptions(x => x.Data.Prefab.MaterialsInfo.Exists(y => y.ColorPreview.Exists(z => z == null, out _), out _), e, "Colors is not unique");
            dbset.CatchExceptions(x => x.Data.Prefab.MaterialsInfo.Exists(y => y.ColorPreview == null, out _), e, "One of sprites is null");
            dbset.CatchExceptions(x => x.Data.Prefab.MaterialsInfo.Exists(y => y.Id != x.Data.Prefab.MaterialsInfoInternal.IndexOf(y), out _), e, "Color id isn't matching array index");
            var sameRI = dbset.Data.FindSame((x, y) => x.Data.Prefab == y.Data.Prefab);
            foreach (var el in sameRI)
                Debug.LogError($"Prefab in {el.name} is not unique", el);

        }
        private void CatchBuyableReferenceExceptions<SO, Data>(DBSOSet<SO> dbset) where SO : BuyableObjectSO<Data> where Data : BuyableObject
        {
            System.Exception e = new();
            dbset.CatchExceptions(x => x.Data.ObjectReference == null, e, "Object Reference is null");

            var same = dbset.Data.FindSame((x, y) => x.Data.ObjectReference == y.Data.ObjectReference);
            foreach (var el in same)
                Debug.LogError($"Object Reference in {el.name} is not unique", el);
        }

#endif //UNITY_EDITOR
    }
}
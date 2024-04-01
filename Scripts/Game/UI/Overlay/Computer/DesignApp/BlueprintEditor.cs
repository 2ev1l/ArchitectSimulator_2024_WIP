using EditorCustom.Attributes;
using Game.DataBase;
using Game.Events;
using Game.Player;
using Game.Serialization.World;
using Game.UI.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;
using Universal.Core;
using Universal.Time;

namespace Game.UI.Overlay.Computer.DesignApp
{
    internal class BlueprintEditor : SingleSceneInstance<BlueprintEditor>
    {
        #region fields & properties
        /// <summary>
        /// Data may be null
        /// </summary>
        public UnityAction OnCurrentDataChanged;
        /// <summary>
        /// Precision is always quarter of cell size
        /// </summary>
        public const float VECTOR_WORKFLOW_PRECISION = 5f;

        public const int CELL_SIZE = 20;
        private static readonly float ScaleStep = 1.25f;
        public static readonly LanguageInfo WarningTextInfo = new(105, TextType.Game);
        public static readonly LanguageInfo IncorrectObjectPlacementTextInfo = new(119, TextType.Game);
        public static readonly LanguageInfo IncorrectRoomsPlacementTextInfo = new(120, TextType.Game);

        [Title("Primary")]
        [SerializeField] private RectTransform blueprintContent;
        [SerializeField] private VectorTimeChanger workflowScrollPosition;

        [SerializeField] private GameObject UILock;
        [SerializeField] private Vector2 blueprintScaleRange = new(0.35f, 4f);
        public BlueprintEditorCreator Creator => creator;
        [SerializeField] private BlueprintEditorCreator creator = new();
        [SerializeField] private DefaultStateMachine floorResultInfoStates;
        [SerializeField] private TextMeshProUGUI floorResultInfoDefaultText;
        [SerializeField] private TextMeshProUGUI floorResultInfoErrorText;
        public BlueprintEditorSelector Selector => selector;
        private readonly BlueprintEditorSelector selector = new();
        public BlueprintEditorRooms Rooms
        {
            get
            {
                rooms ??= new(creator);
                return rooms;
            }
        }
        private BlueprintEditorRooms rooms = null;

        /// <exception cref="System.NullReferenceException"></exception>
        public BlueprintData CurrentData => currentData;
        private BlueprintData currentData = null;
        /// <summary>
        /// Clamped to cell size, represents local workflow coordinates
        /// </summary>
        public Vector2 ViewCenter
        {
            get
            {
                return RoundToCellSize(ViewCenterUnclamped);
            }
        }
        private Vector2 ViewCenterUnclamped
        {
            get
            {
                float scale = blueprintContent.localScale.x;
                float size = blueprintContent.rect.width / 2 * scale;
                Vector2 currentOffset = blueprintContent.anchoredPosition;
                return new Vector2(-size - currentOffset.x, size - currentOffset.y) / scale;
            }
        }
        private bool isInputSystemLocked = false;
        #endregion fields & properties

        #region methods
        #region tests
#if UNITY_EDITOR

        [Button(nameof(Test1))]
        private void Test1()
        {
            BlueprintEditorValidator.CheckFloor(Creator.CurrentFloor, delegate { Debug.Log("Good"); }, null, null);
        }
        [SerializeField] private bool testBool = false;
        private HashSet<List<BlueprintPointInfo>> resultPoints = new(50);
        private void OnDrawGizmos()
        {
            //DrawGizmoTest1();
            if (testBool)
            {
                DrawGizmoTest2();
            }
            else
            {
                DrawGizmoTest3();
            }
        }
        private void DrawGizmoTest3()
        {
            if (!Application.isPlaying) return;
            if (Selector.SelectedElement == null) return;
            RectTransform workflow = Creator.ElementsParent;
            Rooms.UpdateAllBlueprintPoints(ConstructionLocation.Inside);
            HashSet<BlueprintPointInfo> info = Rooms.ElementsPoints;
            Color startColor = Color.red;
            Color endColor = Color.cyan;
            int infoCount = info.Count;
            int currentCount = 0;
            Gizmos.color = endColor;

            List<BlueprintPointInfo> elementsContainsSelected = info.Where(x =>
                    x.AdjacentElements.Contains(Selector.SelectedElement) && x.ConnectedCoordinates.PointLocation == ConstructionLocation.Inside).ToList();

            if (elementsContainsSelected.Count == 0) return;
            infoCount = elementsContainsSelected.Count - 1;
            foreach (BlueprintPointInfo point in Rooms.ElementsPoints)
            {
                float lerp = (float)currentCount / infoCount;
                float size = Mathf.Lerp(0.1f, 0.4f, lerp);
                Gizmos.color = Color.Lerp(startColor, endColor, lerp);

                foreach (var adj in point.AdjacentPoints)
                {
                    //Gizmos.DrawWireSphere(workflow.TransformPoint(adj.LocalWorkflowCoordinates), size);
                }
                Gizmos.color = Color.green;
                GUIStyle style = new();
                style.normal.textColor = Color.cyan;
                Vector3 worldCoord = workflow.TransformPoint(point.LocalWorkflowCoordinates);
                //Gizmos.DrawWireSphere(worldCoord, size);
                Handles.Label(worldCoord, $"{point.LocalWorkflowCoordinates}", style);
                currentCount++;
            }

            return;
        }
        private void DrawGizmoTest2()
        {
            if (!Application.isPlaying) return;
            if (Selector.SelectedElement == null) return;
            RectTransform workflow = Creator.ElementsParent;
            Rooms.UpdateAllBlueprintPoints(ConstructionLocation.Inside);
            BlueprintPointInfo choosedPoint = Rooms.FindAnyPointContainElement(Selector.SelectedElement);
            if (choosedPoint == null) return;
            Rooms.FindAllLoopsByPoints(choosedPoint, this.resultPoints, ConstructionLocation.Inside);
            if (resultPoints.Count == 0) return;
            float minPointsArea = float.MaxValue;
            int minPointsCount = int.MaxValue;
            List<BlueprintPointInfo> minPointsStack = null;
            foreach (List<BlueprintPointInfo> pointsStack in resultPoints)
            {
                int currentPointsCount = pointsStack.Count;
                //if (currentPointsCount > minPointsArea) continue;
                float currentArea = Rooms.CalculateRoomArea(pointsStack);
                if (currentArea > minPointsArea) continue;
                minPointsArea = currentArea;
                minPointsCount = currentPointsCount;
                minPointsStack = pointsStack;
            }

            int currentCount = 0;
            Gizmos.color = Color.red;
            Color[] colors = new Color[] { Color.red, Color.yellow, Color.green, Color.cyan, Color.black };
            float[] sizes = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, };
            Color pointsColor = colors[currentCount % colors.Length];
            float gizmoSize = sizes[currentCount % sizes.Length];
            foreach (BlueprintPointInfo point in minPointsStack)
            {
                Vector3 worldCoord = workflow.TransformPoint(point.LocalWorkflowCoordinates);
                GUIStyle style = new();
                Gizmos.color = pointsColor;
                style.normal.textColor = pointsColor;
                Handles.Label(worldCoord, $"{point.LocalWorkflowCoordinates}", style);
                Gizmos.DrawWireSphere(worldCoord, gizmoSize);

                foreach (BlueprintResourcePlacer placer in point.AdjacentElements)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(placer.transform.position, Vector3.one * 0.2f);
                }
            }
        }
#endif
        private void Load0Data()
        {
            string newBlueprintName = "def";
            GameData.Data.BlueprintsData.TryCreateNewBlueprint(newBlueprintName, new(BuildingType.House, BuildingStyle.American, BuildingFloor.F1, 0, new(BlueprintExistType.Plot, 0)), out _, out _);
            TryLoadData(newBlueprintName);
        }
        #endregion tests

        private void OnEnable()
        {
            FixCamera();
            CheckUILock();
            TryUnlockInputSystem();
            if (currentData == null)
                Load0Data(); //todo remove
        }
        private void OnDisable()
        {
            TryUnlockInputSystem();
        }
        /// <summary>
        /// Fix required for colliders in 2D space
        /// </summary>
        private void FixCamera()
        {
            Transform cameraTransform = CinemachineCamerasController.Instance.MainCameraTransform;
            Vector3 eulerAngles = cameraTransform.eulerAngles;
            if (eulerAngles.x.Approximately(90, 5f) || eulerAngles.x.Approximately(270, 5f))
            {
                eulerAngles.x += 10;
            }
            if (eulerAngles.y.Approximately(90, 5f) || eulerAngles.y.Approximately(270, 5f))
            {
                eulerAngles.y += 10;
            }
            eulerAngles.x = (int)eulerAngles.x;
            eulerAngles.y = (int)eulerAngles.y;
            eulerAngles.z = 0;
            cameraTransform.eulerAngles = eulerAngles;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if UI is locked</returns>
        private bool CheckUILock()
        {
            bool editorDisabled = !CanOpenEditor();
            if (editorDisabled != UILock.activeSelf)
                UILock.SetActive(editorDisabled);
            return editorDisabled;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if editor is modifyable</returns>
        public bool CanOpenEditor()
        {
            return currentData != null;
        }


        /// <summary>
        /// Changes position smoothly with cost of performance
        /// </summary>
        /// <param name="localWorkflowPosition"></param>
        public void FocusToPosition(Vector2 localWorkflowPosition)
        {
            workflowScrollPosition.SetValues(blueprintContent.anchoredPosition, GetFocusPosition(localWorkflowPosition));
            workflowScrollPosition.SetActions(x => blueprintContent.anchoredPosition = x, null, delegate { return !enabled; });
            workflowScrollPosition.Restart(0.5f);
        }
        /// <summary>
        /// Great performance, but it looks confusing for user
        /// </summary>
        /// <param name="localWorkflowPosition"></param>
        public void FocusToPositionImmediate(Vector2 localWorkflowPosition)
        {
            blueprintContent.anchoredPosition = GetFocusPosition(localWorkflowPosition);
        }
        private Vector2 GetFocusPosition(Vector2 localWorkflowPosition)
        {
            float scale = blueprintContent.localScale.x;
            float size = blueprintContent.rect.width / 2;
            return new Vector2(-size - localWorkflowPosition.x, size - localWorkflowPosition.y) * scale;
        }
        public static Vector2 RoundToCellSize(Vector2 localPoint)
        {
            return new(((int)localPoint.x).RoundTo(CELL_SIZE), ((int)localPoint.y).RoundTo(CELL_SIZE));
        }

        [SerializedMethod]
        public void RequestCreateRooms()
        {
            Selector.DeselectCurrentElement(true);
            if (!BlueprintEditorValidator.IsAllObjectsPlacedCorrectly(Creator.CurrentFloor))
            {
                InfoRequest ir = new(null, WarningTextInfo, IncorrectObjectPlacementTextInfo);
                ir.Send();
                return;
            }
            ConfirmRequest cr = new(CreateRooms, null, WarningTextInfo.Text, LanguageLoader.GetTextByType(TextType.Game, 113));
            cr.Send();
        }
        private void CreateRooms()
        {
            Rooms.UpdateAllBlueprintPoints(ConstructionLocation.Inside);
            Rooms.UpdateAllInsideRooms();
            creator.CurrentFloor.SpawnNewRooms(Rooms.RoomsInfo);
        }

        [SerializedMethod]
        public void GenerateTestZones()
        {
            Selector.DeselectCurrentElement(true);
            if (!BlueprintEditorValidator.IsAllObjectsPlacedCorrectly(Creator.CurrentFloor))
            {
                InfoRequest ir = new(null, WarningTextInfo, IncorrectObjectPlacementTextInfo);
                ir.Send();
                return;
            }
            Creator.CurrentFloor.GenerateZonesFor(Creator.CurrentFloor, out _);
            BlueprintEditorValidator.CheckAllObjectsPlacementSmoothly(Creator.CurrentFloor);
        }
        [SerializedMethod]
        public void ClearTestZones()
        {
            Selector.DeselectCurrentElement(true);
            Creator.CurrentFloor.RemoveTestCreatedZones();
            BlueprintEditorValidator.CheckAllObjectsPlacementSmoothly(Creator.CurrentFloor);
        }
        [SerializedMethod]
        public void PreviewBlueprint()
        {
            SaveDataInstantly();
            ConstructionData constructionData = new(currentData, BlueprintEditorSerializer.SerializeRooms(creator));

            //todo
            Debug.Log("Todo");
        }
        [SerializedMethod]
        public void RequestConfirmFinishBuild()
        {
            if (!BlueprintEditorValidator.IsAllResourcesSufficient(Creator, out Dictionary<ResourceData, int> _, out Dictionary<ResourceData, int> _))
            {
                InfoRequest ir = new(null, WarningTextInfo.Text, LanguageLoader.GetTextByType(TextType.Game, 134));
                ir.Send();
                return;
            }
            ConfirmRequest cr = new(OnConfirmedFinishBuild, null, WarningTextInfo.Text, LanguageLoader.GetTextByType(TextType.Game, 133));
            cr.Send();
        }
        private void OnConfirmedFinishBuild()
        {
            SaveDataInstantly();
            if (!GameData.Data.ConstructionsData.TryAdd(currentData, BlueprintEditorSerializer.SerializeRooms(creator)))
            {
                InfoRequest.GetErrorRequest(400).Send();
            }
            //todo remove from project list
            //todo remove resources from warehouse
            floorResultInfoStates.ApplyDefaultState();

            UnloadData();
            Debug.Log("Todo");
        }
        [SerializedMethod]
        public void TryFinishBuild()
        {
            Selector.DeselectCurrentElement(true);
            if (Creator.CurrentBuildingFloor != CurrentData.BuildingData.MaxFloor)
            {
                InfoRequest ir = new(null, WarningTextInfo.Text, LanguageLoader.GetTextByType(TextType.Game, 132));
                ir.Send();
                return;
            }
            TryLockInputSystem();
            floorResultInfoStates.ApplyState(1);
            BlueprintEditorValidator.CheckFloor(Creator.CurrentFloor, OnFinishBuildFloorPassed, OnCheckFloorFailed, OnCheckFloorStageChanged);
        }
        private void OnFinishBuildFloorPassed()
        {
            TryUnlockInputSystem();
            StringBuilder sb = new();
            if (!BlueprintEditorValidator.IsAllResourcesSufficient(Creator, out Dictionary<ResourceData, int> insufficient, out Dictionary<ResourceData, int> allResources))
            {
                sb.Append($"{LanguageLoader.GetTextByType(TextType.Game, 134)}\n\n");
                foreach (var kv in insufficient)
                {
                    ConstructionResourceInfo info = (ConstructionResourceInfo)kv.Key.Info;
                    sb.Append($"{info.NameInfo.Text}-{info.Id:000} : {kv.Key.Count} / {allResources[kv.Key]}\n");
                }
                floorResultInfoStates.ApplyState(3);
                floorResultInfoErrorText.text = sb.ToString();
                return;
            }
            floorResultInfoStates.ApplyState(4);
        }

        [SerializedMethod]
        public void TryIncreaseFloor()
        {
            TryLockInputSystem();
            Selector.DeselectCurrentElement(true);
            floorResultInfoStates.ApplyState(1);
            BlueprintEditorValidator.CheckFloor(Creator.CurrentFloor, OnIncreaseFloorPassed, OnCheckFloorFailed, OnCheckFloorStageChanged);
        }
        private void OnCheckFloorStageChanged(string stage)
        {
            floorResultInfoDefaultText.text = stage;
        }
        private void OnCheckFloorFailed(string result)
        {
            floorResultInfoStates.ApplyState(3);
            floorResultInfoErrorText.text = result;
            TryUnlockInputSystem();
        }
        private void OnIncreaseFloorPassed()
        {
            floorResultInfoStates.ApplyState(2);
            Invoke(nameof(IncreaseFloorInstantly), 0.5f);
        }
        [Button(nameof(IncreaseFloorInstantly))]
        private void IncreaseFloorInstantly()
        {
            TryUnlockInputSystem();
            floorResultInfoStates.ApplyDefaultState();
            Creator.TryIncreaseFloor(CurrentData.BuildingData.MaxFloor);
        }
        private void TryLockInputSystem()
        {
            if (isInputSystemLocked) return;
            isInputSystemLocked = true;
            InputController.Instance.LockInputSystem(0);
        }
        private void TryUnlockInputSystem()
        {
            if (!isInputSystemLocked) return;
            isInputSystemLocked = false;
            InputController.Instance.UnlockInputSystem(0);
        }
        [SerializedMethod]
        public void TryDecreaseFloor()
        {
            Selector.DeselectCurrentElement(true);
            Creator.TryDecreaseFloor();
        }
        [SerializedMethod]
        public void DeselectCurrentElement() => Selector.DeselectCurrentElement(true);
        [SerializedMethod]
        public void ScaleUp()
        {
            Vector2 viewCenter = ViewCenterUnclamped;
            blueprintContent.localScale = Vector3.one * Mathf.Clamp(blueprintContent.localScale.x * ScaleStep, blueprintScaleRange.x, blueprintScaleRange.y);
            FocusToPositionImmediate(viewCenter);
        }
        [SerializedMethod]
        public void ScaleDown()
        {
            Vector2 viewCenter = ViewCenterUnclamped;
            blueprintContent.localScale = Vector3.one * Mathf.Clamp(blueprintContent.localScale.x / ScaleStep, blueprintScaleRange.x, blueprintScaleRange.y);
            FocusToPositionImmediate(viewCenter);
        }
        [SerializedMethod]
        public void ResetScale()
        {
            Vector2 viewCenter = ViewCenterUnclamped;
            blueprintContent.localScale = Vector3.one;
            FocusToPositionImmediate(viewCenter);
        }

        #region Serialization
        public void UnloadData()
        {
            currentData = null;
            ReloadData();
        }
        public bool TryLoadData(string newBlueprintName)
        {
            if (!BlueprintEditorSerializer.TryDeSerialize(newBlueprintName, ref currentData)) return false;
            ReloadData();
            return true;
        }
        private void ReloadData()
        {
            selector.DeselectCurrentElement(true);
            if (CheckUILock())
            {
                creator.ClearAllFloors();
                OnCurrentDataChanged?.Invoke();
                return;
            }
            creator.ReloadAllFloors(currentData);
            OnCurrentDataChanged?.Invoke();
        }
        [SerializedMethod]
        public void RequestSaveData()
        {
            selector.DeselectCurrentElement(true);
            ConfirmRequest cr = new(SaveDataInstantly, null, WarningTextInfo.Text, LanguageLoader.GetTextByType(TextType.Game, 104));
            cr.Send();
        }
        private void SaveDataInstantly()
        {
            BlueprintEditorSerializer.Serialize(creator, ref currentData);
        }
        [SerializedMethod]
        public void RequestRemoveBlueprintElements()
        {
            selector.DeselectCurrentElement(true);
            ConfirmRequest cr = new(delegate
            {
                creator.CurrentFloor.RemoveBlueprintRooms();
                creator.CurrentFloor.RemoveBlueprintResources();
                creator.CurrentFloor.RemoveBlueprintRoomMarkers();
                //don't remove zones
            }, null, WarningTextInfo.Text, LanguageLoader.GetTextByType(TextType.Game, 107));
            cr.Send();
        }
        [SerializedMethod]
        public void RequestReloadBlueprint()
        {
            selector.DeselectCurrentElement(true);
            ConfirmRequest cr = new(ReloadData, null, WarningTextInfo.Text, LanguageLoader.GetTextByType(TextType.Game, 108));
            cr.Send();
        }
        #endregion Serialization

        #endregion methods
    }
}
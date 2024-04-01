using EditorCustom.Attributes;
using Game.DataBase;
using Game.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using Universal.Behaviour;
using Universal.Core;

namespace Game.UI.Overlay.Computer.DesignApp
{
    [RequireComponent(typeof(BlueprintGraphic))]
    internal abstract class BlueprintPlacerBase : CustomButton, IPoolableObject<BlueprintPlacerBase>
    {
        #region fields & properties
        public UnityAction OnMoveEnd;
        /// <summary>
        /// Unmarshalled gameObject
        /// </summary>
        public GameObject GameObject
        {
            get
            {
                _gameObject ??= gameObject;
                return _gameObject;
            }
        }
        private GameObject _gameObject = null;
        /// <summary>
        /// Unmarshalled transform
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }
        private Transform _transform = null;
        public BlueprintGraphic BlueprintGraphic
        {
            get
            {
                if (blueprintGraphic == null)
                    TryGetComponent(out blueprintGraphic);
                return blueprintGraphic;
            }
        }
        [SerializeField] private BlueprintGraphic blueprintGraphic;
        public bool IsMoving => isMoving;
        [SerializeField] private bool isMoving = false;
        public bool IsGoodPlacement
        {
            get => isGoodPlacement;
            protected set => isGoodPlacement = value;
        }
        [SerializeField] private bool isGoodPlacement = true;
        public UnityAction<BlueprintPlacerBase> OnDestroyed { get; set; }
        public bool IsUsing { get; set; }

        [Title("UI")]
        [SerializeField] private Color movingColor = Color.grey;
        [SerializeField] private Color selectedColor = Color.cyan;
        [SerializeField] private Color badPlacementColor = Color.red;
        [SerializeField] private Color goodPlacementColor = Color.white;
        private IEnumerator fastPlacementCoroutine = null;
        private IEnumerator deepPlacementCoroutine = null;

        protected readonly HashSet<BlueprintPlacerBase> adjacentInsideBlueprints = new();
        protected readonly HashSet<BlueprintPlacerBase> adjacentOutsideBlueprints = new();
        protected readonly HashSet<ConnectedPoint> recalculatedInsideCorners = new();
        protected readonly HashSet<ConnectedPoint> recalculatedOutsideCorners = new();

        #region garbage optimization
        //optimizes ~5 kb allocation per frame to 0
        private readonly HashSet<BlueprintPlacerBase> lastCollidedBlueprints = new();
        private readonly HashSet<ConnectedPoint> tempInsideCorners = new();
        private readonly HashSet<ConnectedPoint> tempOutsideCorners = new();
        #endregion garbage optimization

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            OnClicked += TrySelect;
            BlueprintGraphic.OnRectChanged += CheckDeepPlacementSmoothly;
            CheckFastPlacementSmoothly();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            OnClicked -= TrySelect;
            BlueprintGraphic.OnRectChanged -= CheckDeepPlacementSmoothly;
            EndMove();
        }

        protected virtual void TryStartMove()
        {
            isMoving = true;
            BlueprintGraphic.MoveRaycast.SetActive(true);
            TryMove();
        }
        protected virtual void EndMove()
        {
            CancelInvoke(nameof(TryMove));
            isMoving = false;
            BlueprintGraphic.MoveRaycast.SetActive(false);
            UpdateUI();
            OnMoveEnd?.Invoke();
        }
        private void TryMove()
        {
            OnMove();
            if (!isMoving) return;
            Invoke(nameof(TryMove), Time.deltaTime);
        }
        protected virtual void OnMove()
        {
            CheckDeepPlacement();
            RectTransform elementsParent = BlueprintEditor.Instance.Creator.ElementsParent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(elementsParent, Input.mousePosition, CanvasInitializer.OverlayCamera, out Vector2 localPoint);
            TryMoveToCoordinates(localPoint, false);
        }
        /// <summary>
        /// Rounds position to cell size and tries to move. If move wasn't performed, returns to previous position. <br></br>
        /// Choose invoke actions for outer UI update (garbage)
        /// </summary>
        /// <param name="localPoint"></param>
        public void TryMoveToCoordinates(Vector2 localPoint, bool invokeActions)
        {
            Vector2 oldPoint = BlueprintGraphic.Transform.localPosition;
            localPoint = BlueprintEditor.RoundToCellSize(localPoint);
            BlueprintGraphic.ChangeLocalPosition(localPoint);
            if (!BlueprintEditorValidator.CanPlaceEntireElementInsideParent(BlueprintGraphic, BlueprintEditor.Instance.Creator.ElementsParent))
            {
                BlueprintGraphic.ChangeLocalPosition(oldPoint);
            }
            CheckDeepPlacement();
            if (invokeActions)
                OnMoveEnd?.Invoke();
        }
        private void TrySelect()
        {
            if (BlueprintEditor.Instance.Selector.TrySelectElement(this))
            {
                UpdateUI();
                return;
            }
            if (BlueprintEditor.Instance.Selector.SelectedElement != this) return;
            if (isMoving)
                EndMove();
            else
                TryStartMove();
        }
        public void CheckDeepPlacement()
        {
            FastCheckLastCollidedBlueprints();
            CheckFastPlacement(out HashSet<BlueprintPlacerBase> lastCollidedBlueprints, out _);
            lastCollidedBlueprints.SetElementsTo(this.lastCollidedBlueprints);
            FastCheckLastCollidedBlueprints();
        }
        private void FastCheckLastCollidedBlueprints()
        {
            foreach (BlueprintPlacerBase el in lastCollidedBlueprints)
            {
                if (el == null) continue;
                el.CheckFastPlacement();
            }
        }
        public void CheckDeepPlacementSmoothly()
        {
            if (!GameObject.activeInHierarchy) return;
            if (deepPlacementCoroutine != null)
                StopCoroutine(deepPlacementCoroutine);
            deepPlacementCoroutine = CheckDeepPlacementSmoothly_IEnumerator();
            StartCoroutine(deepPlacementCoroutine);
        }
        private IEnumerator CheckDeepPlacementSmoothly_IEnumerator()
        {
            FastCheckLastCollidedBlueprints();
            yield return CheckFastPlacementSmoothly_IEnumerator();
            FastCheckLastCollidedBlueprints();
        }
        public void CheckFastPlacementSmoothly()
        {
            if (!GameObject.activeInHierarchy) return;
            if (fastPlacementCoroutine != null)
                StopCoroutine(fastPlacementCoroutine);
            fastPlacementCoroutine = CheckFastPlacementSmoothly_IEnumerator();
            StartCoroutine(fastPlacementCoroutine);
        }
        private IEnumerator CheckFastPlacementSmoothly_IEnumerator()
        {
            float lerp = 0;
            float waitTime = 0.5f;
            isGoodPlacement = false;
            HashSet<BlueprintPlacerBase> lastCollidedBlueprints = null;
            while (lerp < waitTime)
            {
                yield return null;
                lerp += Time.deltaTime;
                CheckIsGoodPlacement(out lastCollidedBlueprints, out _);
                if (isGoodPlacement)
                {
                    break;
                }
                if (lastCollidedBlueprints.Count > 1)
                {
                    break;
                }
            }
            CheckFastPlacement();
            Invoke(nameof(CheckFastPlacement), waitTime - lerp);
        }
        public void CheckFastPlacement() => CheckFastPlacement(out _, out _);
        private void CheckIsGoodPlacement(out HashSet<BlueprintPlacerBase> lastCollidedBlueprints, out HashSet<BlueprintRoom> lastCollidedRooms)
        {
            isGoodPlacement = !BlueprintEditorValidator.IsElementCollidedWithAny(BlueprintGraphic, out lastCollidedBlueprints, out lastCollidedRooms);
        }

        protected virtual void CheckFastPlacement(out HashSet<BlueprintPlacerBase> lastCollidedBlueprints, out HashSet<BlueprintRoom> lastCollidedRooms)
        {
            CheckIsGoodPlacement(out lastCollidedBlueprints, out lastCollidedRooms);
            lastCollidedBlueprints.SetElementsTo(this.lastCollidedBlueprints);
            CheckAdjacentBlueprints(lastCollidedBlueprints);
            UpdateUI();
        }
        private void CheckAdjacentBlueprints(HashSet<BlueprintPlacerBase> lastCollidedBlueprints)
        {
            adjacentInsideBlueprints.Clear();
            adjacentOutsideBlueprints.Clear();
            BlueprintGraphic.SetInsideLinesLocalCornersTo(recalculatedInsideCorners);
            BlueprintGraphic.SetOutsideLinesLocalCornersTo(recalculatedOutsideCorners);

            foreach (BlueprintPlacerBase placer in lastCollidedBlueprints)
            {
                if (placer is not BlueprintResourcePlacer) continue;
                placer.BlueprintGraphic.SetInsideLinesLocalCornersTo(tempInsideCorners);
                placer.BlueprintGraphic.SetOutsideLinesLocalCornersTo(tempOutsideCorners);
                if (HasAdjacentPoint(recalculatedOutsideCorners, tempOutsideCorners, placer))
                {
                    adjacentOutsideBlueprints.Add(placer);
                }
                if (HasAdjacentPoint(recalculatedInsideCorners, tempInsideCorners, placer))
                {
                    adjacentInsideBlueprints.Add(placer);
                }
            }
        }
        private bool HasAdjacentPoint(HashSet<ConnectedPoint> ownPoints, HashSet<ConnectedPoint> otherPoints, BlueprintPlacerBase otherPlacer)
        {
            float precision = BlueprintEditor.VECTOR_WORKFLOW_PRECISION;
            foreach (ConnectedPoint ownPoint in ownPoints)
            {
                Vector2 workflowOwnCoord = ownPoint.LocalWorfklowCoordinates;
                Vector2 workflowOwnEndCoord = ownPoint.Connected.LocalWorfklowCoordinates;
                foreach (ConnectedPoint otherPoint in otherPoints)
                {
                    Vector2 workflowOtherCoord = otherPoint.LocalWorfklowCoordinates;
                    Vector2 workflowOtherEndCoord = otherPoint.Connected.LocalWorfklowCoordinates;
                    if (workflowOwnCoord.Approximately(workflowOtherCoord, precision)) return true;
                    if (workflowOwnCoord.PointOnLine2D(workflowOtherCoord, workflowOtherEndCoord, precision)) return true;
                    if (workflowOtherCoord.PointOnLine2D(workflowOwnCoord, workflowOwnEndCoord, precision)) return true;
                }
            }
            return false;
        }
        public void SetLastCollidedBlueprintsTo(HashSet<BlueprintPlacerBase> newCollidedBlueprints) => lastCollidedBlueprints.SetElementsTo(newCollidedBlueprints);
        public void SetAdjacentInsideBlueprintsTo(HashSet<BlueprintPlacerBase> newAdjacentInsideElements) => adjacentInsideBlueprints.SetElementsTo(newAdjacentInsideElements);
        public void SetAdjacentOutsideBlueprintsTo(HashSet<BlueprintPlacerBase> newAdjacentOutsideElements) => adjacentInsideBlueprints.SetElementsTo(newAdjacentOutsideElements);
        public void ReplaceUI(BlueprintPlacerBase newUI)
        {
            movingColor = newUI.movingColor;
            selectedColor = newUI.selectedColor;
            badPlacementColor = newUI.badPlacementColor;
            goodPlacementColor = newUI.goodPlacementColor;
            UpdateUI();
        }
        /// <summary>
        /// Calls each time while moving
        /// </summary>
        protected virtual void UpdateUI()
        {
            Color currentColor = Color.white;
            bool isSelected = BlueprintEditor.Instance.Selector.SelectedElement == this;
            if (isSelected)
            {
                currentColor = selectedColor;
            }
            if (isMoving)
            {
                currentColor = movingColor;
            }
            currentColor = Color.Lerp(currentColor, isGoodPlacement ? (isSelected ? selectedColor : goodPlacementColor) : badPlacementColor, 0.6f);

            BlueprintGraphic.ChangeGraphicsColor(currentColor);
        }
        public void OnDeselected()
        {
            EndMove();
            CheckDeepPlacement();
        }
        public void OnSelected()
        {
            EndMove();
            CheckDeepPlacement();
        }

        public void DisableObject()
        {
            if (GameObject.activeSelf)
                GameObject.SetActive(false);
            IsUsing = false;
        }
        public BlueprintPlacerBase InstantiateThis(Transform parentForSpawn)
        {
            BlueprintPlacerBase obj = Instantiate(this, parentForSpawn);
            return FakeInstantiateAgain(obj);
        }
        public BlueprintPlacerBase FakeInstantiateAgain(BlueprintPlacerBase obj)
        {
            GameObject go = obj.GameObject;
            if (!go.activeSelf)
                go.SetActive(true);
            return obj;
        }
        public abstract void RemoveBlueprint();
        public abstract void CloneBlueprint();
        #endregion methods
    }
}
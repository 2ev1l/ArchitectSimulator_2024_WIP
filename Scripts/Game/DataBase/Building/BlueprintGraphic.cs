using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Universal.Behaviour;
using Universal.Core;

namespace Game.DataBase
{
    public class BlueprintGraphic : StaticPoolableObject
    {
        #region fields & properties
        /// <summary>
        /// 1 rect width = 1 cm * CmToRectScale
        /// </summary>
        public static readonly int CentimetersToRectScale = 2;
        public UnityAction OnRectChanged;
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
        private Transform _transform;
        public RectTransform ChildBase
        {
            get => childBase;
            protected set => childBase = value;
        }
        [SerializeField] private RectTransform childBase;
        public RectTransform RectCollisionCheck
        {
            get
            {
                if (rectCollisionCheck == null)
                    rectCollisionCheck = ChildBase;
                return rectCollisionCheck;
            }
            protected set => rectCollisionCheck = value;
        }
        [SerializeField] private RectTransform rectCollisionCheck;
        public BoxCollider2D BaseCollider
        {
            get => baseCollider;
            protected set => baseCollider = value;
        }
        [SerializeField] private BoxCollider2D baseCollider;
        public int RotationScale => rotationScale;
        [SerializeField][ReadOnly] private int rotationScale = 0;
        protected Graphic[] ChildGraphics
        {
            get => childGraphics;
            set => childGraphics = value;
        }
        [SerializeField] private Graphic[] childGraphics;
        protected RectLine[] OutsideLines
        {
            get => outsideLines;
            set => outsideLines = value;
        }
        [SerializeField] private RectLine[] outsideLines;
        protected RectLine[] InsideLines
        {
            get => insideLines;
            set => insideLines = value;
        }
        [SerializeField] private RectLine[] insideLines;
        public GameObject MoveRaycast => moveRaycast;
        [SerializeField] private GameObject moveRaycast;

        protected HashSet<ConnectedPoint> OutsideLinesCorners
        {
            get
            {
                CheckOutsideLinesCorners();
                return outsideLinesCorners;
            }
        }
        private HashSet<ConnectedPoint> outsideLinesCorners;
        protected HashSet<ConnectedPoint> InsideLinesCorners
        {
            get
            {
                CheckInsideLinesCorners();
                return insideLinesCorners;
            }
        }
        private HashSet<ConnectedPoint> insideLinesCorners;
        private Vector2[] EntireLocalCorners
        {
            get
            {
                entireLocalCorners ??= GetNewEntireLocalCornersArray();
                if (entireLocalCorners.Length < 4)
                    entireLocalCorners = GetNewEntireLocalCornersArray();
                return entireLocalCorners;
            }
        }
        private Vector2[] entireLocalCorners;
        /// <summary>
        /// Center of object with parent as workflow
        /// </summary>
        public Vector2 LocalCenter => entireLocalCorners[4];
        private Color lastGraphicsColor = Color.clear;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Use this instead of <see cref="Transform.localPosition"/>. <br></br>
        /// High-cost operation but with UI update.
        /// </summary>
        public void ChangeLocalPosition(Vector2 newLocalPosition)
        {
            Transform.localPosition = newLocalPosition;
            UpdateAllCorners();
        }
        public void ChangeGraphicsColor(Color newColor)
        {
            if (lastGraphicsColor.Equals(newColor)) return;
            lastGraphicsColor = newColor;
            for (int i = 0; i < childGraphics.Length; ++i)
            {
                if (childGraphics[i] == null) continue;
                newColor.a = childGraphics[i].color.a;
                childGraphics[i].color = newColor;
            }
        }
        /// <summary>
        /// Use this instead of <see cref="Transform.Rotate"/>
        /// </summary>
        public virtual void Rotate() => RotateTo(rotationScale + 1);
        /// <summary>
        /// Use this instead of <see cref="Transform.Rotate"/>
        /// </summary>
        public void ResetRotation() => RotateTo(0);
        /// <summary>
        /// Use this instead of <see cref="Transform.Rotate"/>. <br></br>
        /// High-cost method with entire recalculations. If you don't want to leak performance for multiple objects, then use <see cref="RotateToWithMultipleInstantiating(int)"/>
        /// </summary>
        public virtual void RotateTo(int scale)
        {
            RotateToWithMultipleInstantiating(scale);
            OnRectChanged?.Invoke();
        }
        public void RotateToWithMultipleInstantiating(int scale)
        {
            RotateToWithoutActionsUpdate(scale);
            UpdateAllCorners();
        }
        private void RotateToWithoutActionsUpdate(int scale)
        {
            rotationScale = scale % 4;
            transform.localEulerAngles = GetLocalEulerAngles(rotationScale, Transform.localEulerAngles);
        }
        public static Vector3 GetLocalEulerAngles(int rotationScale, Vector3 ownEulerAngles) => new(ownEulerAngles.x, ownEulerAngles.y, (int)(90 * rotationScale));
        private void CheckOutsideLinesCorners()
        {
            if (outsideLinesCorners != null) return;
            outsideLinesCorners = new();
            UpdateLinesCorners(outsideLines, outsideLinesCorners);
        }
        private void CheckInsideLinesCorners()
        {
            if (insideLinesCorners != null) return;
            insideLinesCorners = new();
            UpdateLinesCorners(insideLines, insideLinesCorners);
        }
        public void ForceCollisionUpdate() => UpdateAllCorners();
        private void UpdateAllCorners()
        {
            RecalculateEntireLocalCornersForArray(EntireLocalCorners);
            UpdateLinesCorners(outsideLines, OutsideLinesCorners);
            UpdateLinesCorners(insideLines, InsideLinesCorners);
        }
        private void GetUICornerParams(out Vector2 widthScale, out Vector2 heightScale, out float rectWidth, out float rectHeight) => GetAnyCornerParams(ChildBase, out widthScale, out heightScale, out rectWidth, out rectHeight);
        private void GetCollisionCornerParams(out Vector2 widthScale, out Vector2 heightScale, out float rectWidth, out float rectHeight) => GetAnyCornerParams(RectCollisionCheck, out widthScale, out heightScale, out rectWidth, out rectHeight);

        private void GetAnyCornerParams(RectTransform rectTransorm, out Vector2 widthScale, out Vector2 heightScale, out float rectWidth, out float rectHeight)
        {
            widthScale = Vector2.right;
            heightScale = Vector2.up;
            Rect rectFromTranfsorm = rectTransorm.rect;
            rectWidth = rectFromTranfsorm.width;
            rectHeight = rectFromTranfsorm.height;
            switch (rotationScale)
            {
                case 0: return;
                case 1:
                    rectWidth = rectFromTranfsorm.height;
                    rectHeight = rectFromTranfsorm.width;
                    heightScale = Vector2.down;
                    return;
                case 2:
                    widthScale = Vector2.left;
                    heightScale = Vector2.down;
                    return;
                case 3:
                    rectWidth = rectFromTranfsorm.height;
                    rectHeight = rectFromTranfsorm.width;
                    heightScale = Vector2.up;
                    widthScale = Vector2.left;
                    return;
                default: return;
            }
        }
        private void UpdateLinesCorners(RectLine[] lines, HashSet<ConnectedPoint> corners)
        {
            Transform ownTransform = Transform;
            corners.Clear();
            Transform parent = Transform.parent;
            GetUICornerParams(out Vector2 widthScale, out Vector2 heightScale, out _, out _);
            float childWidth = ChildBase.rect.width;
            float childHeight = ChildBase.rect.height;
            Vector2 precalculatedWidth = widthScale * (childWidth / 2);
            Vector2 precalculatedHeight = heightScale * (childHeight / 2);
            for (int i = 0; i < lines.Length; ++i)
            {
                RectLine line = lines[i];
                if (line == null) continue;
                if (!line.GameObject.activeInHierarchy) continue;
                GetLineCorners(line, childHeight, childWidth, precalculatedHeight, precalculatedWidth, parent, out Vector2 baseLineStart, out Vector2 baseLineEnd, out Vector2 workflowStart, out Vector2 worfklowEnd);
                line.UpdateCorners(baseLineStart, baseLineEnd, workflowStart, worfklowEnd);
                corners.Add(line.StartPoint);
                corners.Add(line.EndPoint);
            }
        }
        private void GetLineCorners(RectLine line, float childHeight, float childWidth, Vector2 precalculatedHeight, Vector2 precalculatedWidth, Transform parent, out Vector2 start, out Vector2 end, out Vector2 workflowStart, out Vector2 worfklowEnd)
        {
            Vector2 finalScale = Vector2.zero;
            Vector2 localPos = Transform.InverseTransformPoint(line.Transform.position);
            if (line.Vertical)
            {
                float heightDownscale = childHeight / (float)line.RectTransform.rect.height;
                finalScale = precalculatedHeight / heightDownscale;
            }
            else
            {
                float widthDownscale = childWidth / (float)line.RectTransform.rect.width;
                finalScale = precalculatedWidth / widthDownscale;
            }
            start = localPos + finalScale;
            end = localPos - finalScale;
            workflowStart = parent.InverseTransformPoint(Transform.TransformPoint(start));
            worfklowEnd = parent.InverseTransformPoint(Transform.TransformPoint(end));
        }
        /// <summary>
        /// New Corners will be cleared <br></br>
        /// More efficient than <see cref="IEnumerable"/> but still has a big impact on performance
        /// </summary>
        /// <param name="newCorners"></param>
        public void SetInsideLinesLocalCornersTo(HashSet<ConnectedPoint> newCorners) => InsideLinesCorners.SetElementsTo(newCorners);
        /// <summary>
        /// New Corners will be cleared <br></br>
        /// More efficient than <see cref="IEnumerable"/> but still has a big impact on performance
        /// </summary>
        /// <param name="newCorners"></param>
        public void SetOutsideLinesLocalCornersTo(HashSet<ConnectedPoint> newCorners) => OutsideLinesCorners.SetElementsTo(newCorners);

        /// <summary>
        /// 0 => Top Left; <br></br>
        /// 1 => Top Right; <br></br>
        /// 2 => Bottom Right; <br></br>
        /// 3 => Bottom Left; <br></br>
        /// 4 => Center; <br></br>
        /// Local collision with parent of main placer
        /// </summary>
        public void SetEntrieLocalCollisionCorners(Vector2[] fivePointArray)
        {
            fivePointArray[0] = EntireLocalCorners[0];
            fivePointArray[1] = EntireLocalCorners[1];
            fivePointArray[2] = EntireLocalCorners[2];
            fivePointArray[3] = EntireLocalCorners[3];
            fivePointArray[4] = EntireLocalCorners[4];
        }
        /// <summary>
        /// 0 => Top Left; <br></br>
        /// 1 => Top Right; <br></br>
        /// 2 => Bottom Right; <br></br>
        /// 3 => Bottom Left; <br></br>
        /// 4 => Center; <br></br>
        /// Local collision with parent of main placer
        /// </summary>
        public void SetEntrieLocalCollisionCorners(List<Vector2> fivePointArray)
        {
            fivePointArray.Clear();
            fivePointArray.Add(EntireLocalCorners[0]);
            fivePointArray.Add(EntireLocalCorners[1]);
            fivePointArray.Add(EntireLocalCorners[2]);
            fivePointArray.Add(EntireLocalCorners[3]);
            fivePointArray.Add(EntireLocalCorners[4]);
        }
        private Vector2[] GetNewEntireLocalCornersArray()
        {
            Vector2[] array = new Vector2[5];
            RecalculateEntireLocalCornersForArray(array);
            return array;
        }
        private void RecalculateEntireLocalCornersForArray(Vector2[] fivePointArray)
        {
            GetCollisionCornerParams(out Vector2 widthScale, out Vector2 heightScale, out float rectWidth, out float rectHeight);
            Vector2 localPos = Transform.localPosition;
            fivePointArray[0] = (localPos);
            fivePointArray[1] = (localPos + widthScale * rectWidth);
            fivePointArray[2] = (localPos + widthScale * rectWidth - heightScale * rectHeight);
            fivePointArray[3] = (localPos - heightScale * rectHeight);
            fivePointArray[4] = localPos + (widthScale * rectWidth) / 2 - (heightScale * rectHeight) / 2;
        }

        public virtual void Replace(BlueprintGraphic newElement)
        {
            ReplaceWithMultipleInstantiating(newElement);
            UpdateAllCorners();
            OnRectChanged?.Invoke();
        }
        public virtual void ReplaceWithMultipleInstantiating(BlueprintGraphic newElement)
        {
            if (ChildBase.sizeDelta != newElement.ChildBase.sizeDelta)
                ChildBase.sizeDelta = newElement.ChildBase.sizeDelta;
            if (ChildBase.localPosition != newElement.ChildBase.localPosition)
                ChildBase.localPosition = newElement.ChildBase.localPosition;
            if (RectCollisionCheck.sizeDelta != newElement.rectCollisionCheck.sizeDelta)
                RectCollisionCheck.sizeDelta = newElement.rectCollisionCheck.sizeDelta;
            if (RectCollisionCheck.localPosition != newElement.rectCollisionCheck.localPosition)
                RectCollisionCheck.localPosition = newElement.rectCollisionCheck.localPosition;
            BaseCollider.size = newElement.baseCollider.size;
            //causes a big performance leak
            for (int i = 0; i < newElement.ChildGraphics.Length; ++i)
            {
                if (ChildGraphics.Length <= i) break;
                if (ChildGraphics[i] == null) continue;
                if (newElement.ChildGraphics[i] == null) continue;
                RectTransform myChildRect = ChildGraphics[i].rectTransform;
                RectTransform newChildRect = newElement.ChildGraphics[i].rectTransform;
                ChildGraphics[i].color = newElement.ChildGraphics[i].color;
                if (myChildRect.localPosition != newChildRect.localPosition)
                    myChildRect.localPosition = newChildRect.localPosition;
                if (myChildRect.sizeDelta != newChildRect.sizeDelta)
                    myChildRect.sizeDelta = newChildRect.sizeDelta;
                if (myChildRect.anchorMin != newChildRect.anchorMin)
                    myChildRect.anchorMin = newChildRect.anchorMin;
                if (myChildRect.anchorMax != newChildRect.anchorMax)
                    myChildRect.anchorMax = newChildRect.anchorMax;
                if (myChildRect.anchoredPosition != newChildRect.anchoredPosition)
                    myChildRect.anchoredPosition = newChildRect.anchoredPosition;
            }
        }
        /// <summary>
        /// Warning, scale doesn't affect on element points position and other calculations!
        /// </summary>
        public void ClampScaleWidthAndHeight(float maxWidth, float maxHeight)
        {

            if (maxHeight >= ChildBase.sizeDelta.y)
            {
                ClampScaleWidth(maxWidth);
                return;
            }
            if (maxWidth >= ChildBase.sizeDelta.x)
            {
                ClampScaleHeight(maxHeight);
                return;
            }
            float min = Mathf.Min(maxHeight / ChildBase.sizeDelta.y, maxWidth / ChildBase.sizeDelta.x);
            Transform.localScale = Vector3.one * min;
            return;
            Transform.localScale = Vector3.up * (maxHeight / ChildBase.sizeDelta.y) + Vector3.right * (maxWidth / ChildBase.sizeDelta.x);
        }
        /// <summary>
        /// Warning, scale doesn't affect on element points position and other calculations!
        /// </summary>
        public void ClampScaleHeight(float maxHeight)
        {
            if (maxHeight >= ChildBase.sizeDelta.y)
            {
                Transform.localScale = Vector3.one;
                return;
            }
            Transform.localScale = Vector3.one * (maxHeight / ChildBase.sizeDelta.y);
        }
        /// <summary>
        /// Warning, scale doesn't affect on element points position and other calculations!
        /// </summary>
        public void ClampScaleWidth(float maxWidth)
        {
            if (maxWidth >= ChildBase.sizeDelta.x)
            {
                Transform.localScale = Vector3.one;
                return;
            }
            Transform.localScale = Vector3.one * (maxWidth / ChildBase.sizeDelta.x);
        }
        /// <summary>
        /// If you don't want to leak performance for multiple objects, then use <see cref="ChangeSizeWithMultipleInstantiating"/>
        /// </summary>
        /// <param name="rectWidth"></param>
        /// <param name="rectHeight"></param>
        public virtual void ChangeSize(float rectWidth, float rectHeight)
        {
            ChangeSizeWithMultipleInstantiating(rectWidth, rectHeight);
            UpdateAllCorners();
            OnRectChanged?.Invoke();
        }
        public void ChangeSizeWithMultipleInstantiating(float rectWidth, float rectHeight)
        {
            int lastRotation = rotationScale;
            RotateToWithoutActionsUpdate(0);
            Vector2 newSize = new(rectWidth, rectHeight);
            ChildBase.sizeDelta = newSize;
            ChildBase.localPosition = new(rectWidth / 2, -rectHeight / 2, ChildBase.localPosition.z);
            BaseCollider.size = ChildBase.sizeDelta;
            RotateToWithoutActionsUpdate(lastRotation);
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Universal.Behaviour;
using Universal.Core;

namespace Game.DataBase
{
    /// <summary>
    /// Graphic can't be moved manually and may have a different behaviour related to default <see cref="BlueprintGraphic"/>
    /// </summary>
    public abstract class PolygonBlueprintGraphic : StaticPoolableObject
    {
        #region fields & properties
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
        public PolygonCollider2D PolygonCollider => polygonCollider;
        [SerializeField] private PolygonCollider2D polygonCollider;
        public SpriteShapeRenderer SpriteShapeRenderer => spriteShapeRenderer;
        [Title("UI")][SerializeField] private SpriteShapeRenderer spriteShapeRenderer;
        public SpriteShapeController SpriteShapeController => spriteShapeController;
        [SerializeField] private SpriteShapeController spriteShapeController;
        [SerializeField] private Color goodPlacementColor = Color.white;
        [SerializeField] private Color badPlacementColor = Color.white;
        private IEnumerator activeCoroutine = null;
        private bool isColliderBaked = false;
        public bool IsGoodPlacement
        {
            get => isGoodPlacement;
            set => isGoodPlacement = value;
        }
        [SerializeField] private bool isGoodPlacement = false;

        #region garbage optimization
        public IReadOnlyList<Vector2> LocalTexturePoints => localTexturePoints;
        protected readonly List<Vector2> localTexturePoints = new();
        protected readonly List<Vector2> colldierPoints = new();
        protected readonly HashSet<BlueprintGraphic> lastCatchedGraphics = new();
        protected readonly HashSet<PolygonBlueprintGraphic> lastCatchedPolygons = new();
        private readonly List<Collider2D> lastCatchedColliders = new();
        #endregion garbage optimization

        #endregion fields & properties

        #region methods
        /// <summary>
        /// Use this method to initialize graphic
        /// </summary>
        public void UpdateGraphic(List<Vector2> localTexturePoints, bool updateCollision)
        {
            isColliderBaked = false;
            SetTextureCoordinates(localTexturePoints);
            GenerateUI();
            if (updateCollision)
                UpdateCollisionSmoothly();
            UpdateUI();
        }
        protected void SetTextureCoordinates(List<Vector2> localTexturePoints)
        {
            localTexturePoints.SetElementsTo(this.localTexturePoints);
        }
        public void SetColliderPointsTo(List<Vector2> colliderPoints)
        {
            polygonCollider.GetPath(0, colliderPoints);
        }
        public void SetTextureCoordinatesTo(List<Vector2> newTextureCoordinates)
        {
            localTexturePoints.SetElementsTo(newTextureCoordinates);
        }
        /// <summary>
        /// You should update <see cref="localTexturePoints"/> if it's required
        /// Invokes before UI generate
        /// </summary>
        protected abstract void UpdateTextureCoordinates();

        public void UpdateCollisionSmoothly()
        {
            if (activeCoroutine != null)
                StopCoroutine(activeCoroutine);
            if (!GameObject.activeInHierarchy) return;
            activeCoroutine = UpdateCollisionSmoothly_IEnumerator();
            StartCoroutine(activeCoroutine);
        }
        private IEnumerator UpdateCollisionSmoothly_IEnumerator()
        {
            if (!isColliderBaked)
            {
                isColliderBaked = true;
                spriteShapeController.BakeCollider();
            }
            float maxLerp = 0.5f;
            float lerp = 0f;
            while (lerp < maxLerp)
            {
                yield return null;
                GetCollidedBlueprints(polygonCollider);
                if (lastCatchedGraphics.Count > 0 || lastCatchedPolygons.Count > 0) break;
                lerp += Time.deltaTime;
            }
            UpdateCollisionInstant();
            Invoke(nameof(UpdateCollisionInstant), maxLerp - lerp);
        }

        /// <summary>
        /// Invokes after collision update before placement check call
        /// </summary>
        protected abstract void OnCollisionUpdated();
        /// <summary>
        /// Placement invokes after collision found and just before UI update
        /// </summary>
        protected abstract void UpdatePlacement();
        /// <summary>
        /// UI updates after the placement updated
        /// </summary>
        protected virtual void UpdateUI()
        {
            spriteShapeRenderer.color = IsGoodPlacement ? goodPlacementColor : badPlacementColor;
        }
        public void UpdateCollisionInstant()
        {
            GetCollidedBlueprints(polygonCollider);
            OnCollisionUpdated();
            UpdatePlacement();
            UpdateUI();
        }
        protected void SetTexureAsCollider()
        {
            polygonCollider.GetPath(0, colldierPoints);
            GenerateTexture(colldierPoints, false);
        }
        protected virtual void GenerateUI()
        {
            UpdateTextureCoordinates();
            GenerateTexture(localTexturePoints);
            UpdateCollider(colldierPoints);
        }
        private void UpdateCollider(List<Vector2> path)
        {
            polygonCollider.pathCount = 1;
            polygonCollider.SetPath(0, path);
        }
        /// <summary>
        /// Points should be with coordinates referencing to local workflow
        /// </summary>
        /// <param name="points"></param>
        private void GenerateTexture(List<Vector2> points, bool generateColliderPoints = true)
        {
            colldierPoints.Clear();
            int pointsCount = points.Count;
            float scale = Transform.localScale.x;
            int i = 0;
            Spline spline = spriteShapeController.spline;
            spline.Clear();

            for (int pi = 0; pi < pointsCount; ++pi)
            {
                Vector2 point = points[pi];
                try { spline.InsertPointAt(i, point / scale); }
                catch
                {
                    Debug.LogError($"E : {spline.GetPosition(i - 1)} => {point}");
                    continue;
                }
                ++i;
            }
            if (!generateColliderPoints) return;
            int splinesCount = spline.GetPointCount();
            for (int k = 0; k < splinesCount; ++k)
            {
                colldierPoints.Add(spline.GetPosition(k));
            }
        }
        private void GetCollidedBlueprints(PolygonCollider2D collider)
        {
            lastCatchedGraphics.Clear();
            lastCatchedPolygons.Clear();
            Physics2D.OverlapCollider(collider, lastCatchedColliders);
            int collidersCount = lastCatchedColliders.Count;
            for (int i = 0; i < collidersCount; ++i)
            {
                Collider2D el = lastCatchedColliders[i];
                if (el == null) continue;
                Transform elParent = el.transform.parent;
                if (elParent == null) continue;
                GameObject elGameObject = el.gameObject;
                if (elGameObject.TryGetComponent(out PolygonBlueprintGraphic collidedPolygon))
                {
                    this.lastCatchedPolygons.Add(collidedPolygon);
                    continue;
                }
                GameObject elGameObjectParent = elParent.gameObject;
                if (!elGameObjectParent.TryGetComponent(out BlueprintGraphic collidedGraphic))
                {
                    this.lastCatchedGraphics.Add(collidedGraphic);
                }
            }
        }
        #endregion methods
    }
}
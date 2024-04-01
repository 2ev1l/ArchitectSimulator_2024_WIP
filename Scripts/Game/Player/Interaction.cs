using DebugStuff;
using EditorCustom.Attributes;
using Game.Events;
using UnityEngine;
using System;

namespace Game.Player
{
    public class Interaction : MonoBehaviour
    {
        #region fields & properties
        public InteractableObject LastCatchedObject
        {
            get => lastCatchedObject;
            set => SetCatchedObject(value);
        }
        private InteractableObject lastCatchedObject = null;
        [SerializeField][Range(0f, 10f)] private float interactDistance = 3f;
        [SerializeField][Range(0f, 1f)] private float interactRadius = 0.1f;
        [SerializeField] private LayerMask interactableMask;
        [SerializeField] private LayerMask obstructionMask;
        private static readonly Vector3[] boundsCornersScale = new Vector3[]
        {
            new(-1, -1, -1),
            new(-1, -1, 1),
            new(-1, 1, -1),
            new(-1, 1, 1),
            new(1, -1, -1),
            new(1, -1, 1),
            new(1, 1, -1),
            new(1, 1, 1),
        };

        #region garbage optimization
        private Collider[] catchedObjects = new Collider[10];
        private Plane[] currentFrustrumPlanes = new Plane[6];
        private Vector3[] boundsCorners = new Vector3[8];
        #endregion garbage optimization
        #endregion fields & properties

        #region methods
        private Ray UpdateObstructionRay(Vector3 corner)
        {
            Ray obstructionRay = new()
            {
                origin = CinemachineCamerasController.Instance.MainCameraTransform.position
            };
            obstructionRay.direction = corner - obstructionRay.origin;
            return obstructionRay;
        }
        private Ray GetInteractRay()
        {
            Transform mainCameraTransform = CinemachineCamerasController.Instance.MainCameraTransform;
            Ray interactRay = new()
            {
                origin = mainCameraTransform.position,
                direction = mainCameraTransform.forward
            };
            return interactRay;
        }
        public InteractableObject CatchObject()
        {
            Array.Clear(catchedObjects, 0, catchedObjects.Length);
            Ray interactRay = GetInteractRay();
            Vector3 sphereOrigin = interactRay.origin + interactRay.direction * interactDistance / 2;
            float sphereRadius = Mathf.Max(interactDistance / 2f, interactRadius);

            if (Physics.Raycast(interactRay, out RaycastHit hit, interactDistance, interactableMask, QueryTriggerInteraction.Ignore))
            {
                sphereOrigin = hit.point;
                sphereRadius = interactRadius;
            }

            Physics.OverlapSphereNonAlloc(sphereOrigin, sphereRadius, catchedObjects, interactableMask, QueryTriggerInteraction.Ignore);
            GeometryUtility.CalculateFrustumPlanes(CinemachineCamerasController.Instance.MainCamera, currentFrustrumPlanes);
            InteractableObject tempChoosedObject = null;
            float minDistanceToCamera = Mathf.Infinity;
            foreach (Collider el in catchedObjects)
            {
                if (el == null) continue;
                if (!el.TryGetComponent(out InteractableObject io)) continue;
                if (!io.enabled) continue;
                if (!GeometryUtility.TestPlanesAABB(currentFrustrumPlanes, io.Render.bounds)) continue;
                float distanceToCamera = Vector3.Distance(el.ClosestPoint(sphereOrigin), sphereOrigin);

                if (distanceToCamera > minDistanceToCamera) continue;
                if (IsObstructed(io.Render, el)) continue;
                tempChoosedObject = io;
                minDistanceToCamera = distanceToCamera;
            }
            return tempChoosedObject;
        }
        private bool IsObstructed(Renderer target, Collider collider)
        {
            Bounds bounds = target.bounds;
            Vector3[] corners = GetBoundsCorners(bounds);
            float rayDistance = 0;
            foreach (Vector3 corner in corners)
            {
                Ray obstructionRay = UpdateObstructionRay(corner);
                rayDistance = Vector3.Distance(corner, obstructionRay.origin);
                if (!Physics.Raycast(obstructionRay, out RaycastHit hit, rayDistance, obstructionMask, QueryTriggerInteraction.Ignore))
                {
                    return false;
                }
                else
                {
                    if (hit.collider == collider) return false;
                }
            }
            return true;
        }
        private Vector3[] GetBoundsCorners(Bounds bounds)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            for (int i = 0; i < 8; ++i)
            {
                Vector3 currentScale = boundsCornersScale[i];
                boundsCorners[i] = center + new Vector3(currentScale.x * extents.x, currentScale.y * extents.y, currentScale.z * extents.z);
            }

            return boundsCorners;
        }
        private void SetCatchedObject(InteractableObject value)
        {
            if (value == null)
            {
                if (lastCatchedObject != null)
                    lastCatchedObject.DeSelect();
                lastCatchedObject = null;
                return;
            }

            if (lastCatchedObject != value)
            {
                if (lastCatchedObject != null)
                    lastCatchedObject.DeSelect();

                lastCatchedObject = value;
                lastCatchedObject.Select();
            }
        }
        public bool TryInteract()
        {
            if (LastCatchedObject == null) return false;
            LastCatchedObject.Interact();
            return true;
        }
        private void Update()
        {
            LastCatchedObject = CatchObject();
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool debugAlways = false;

        private void OnDrawGizmosSelected()
        {
            if (!doDebug) return;
            if (debugAlways) return;
            DebugDraw();
        }
        private void OnDrawGizmos()
        {
            if (!doDebug) return;
            if (!debugAlways) return;
            DebugDraw();
        }

        private void DebugDraw()
        {
            Ray interactRay = GetInteractRay();
            Vector3 sphereOrigin = interactRay.origin + interactRay.direction * interactDistance / 2;
            float sphereRadius = Mathf.Max(interactDistance / 2, interactRadius);

            if (Physics.Raycast(interactRay, out RaycastHit hit, interactDistance, interactableMask, QueryTriggerInteraction.Ignore))
            {
                sphereOrigin = hit.point;
                sphereRadius = interactRadius;
            }
            Debug.DrawLine(interactRay.origin, sphereOrigin, Color.cyan);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(sphereOrigin, sphereRadius);


            Collider[] catched = Physics.OverlapSphere(sphereOrigin, sphereRadius, interactableMask, QueryTriggerInteraction.Ignore);
            Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            float minDistanceToCamera = Mathf.Infinity;
            InteractableObject choosedObject = null;
            foreach (Collider el in catched)
            {
                if (!el.TryGetComponent(out InteractableObject io)) continue;
                if (!GeometryUtility.TestPlanesAABB(frustrumPlanes, io.Render.bounds)) continue;
                float distanceToCamera = Vector3.Distance(el.ClosestPoint(sphereOrigin), sphereOrigin);

                if (distanceToCamera > minDistanceToCamera) continue;
                Gizmos.DrawLineList(GetBoundsCorners(io.Render.bounds));
                if (IsObstructed(io.Render, el)) continue;
                choosedObject = io;
                minDistanceToCamera = distanceToCamera;
            }
        }

#endif //UNITY_EDITOR

    }
}
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugStuff
{
    public class DrawGizmo : MonoBehaviour
    {
#if UNITY_EDITOR
        #region fields & properties
        public GizmoType Gizmo
        {
            get => gizmoType;
            set => gizmoType = value;
        }
        [SerializeField] private GizmoType gizmoType = GizmoType.WireSphere;
        public float Radius
        {
            get => radius;
            set => radius = value;
        }
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireSphere | GizmoType.Sphere)][Min(0)] private float radius = 1f;
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireCube | GizmoType.Cube)] private Vector3 scale;
        public Mesh Mesh
        {
            get => mesh;
            set => mesh = value;
        }
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireMesh | GizmoType.Mesh)] private Mesh mesh;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private bool drawAlways = true;
        [SerializeField] private bool useThisPosition = true;
        [SerializeField] private bool useThisRotation = true;
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireMesh | GizmoType.Mesh)] private bool useThisScale = true;

        private Transform ReferencePositionTransform => useThisPosition ? transform : referencePositionTransform;
        [SerializeField][DrawIf(nameof(useThisPosition), false)] private Transform referencePositionTransform;
        private Transform ReferenceRotationTransform => useThisRotation ? transform : referenceRotationTransform;
        [SerializeField][DrawIf(nameof(useThisRotation), false)] private Transform referenceRotationTransform;
        private Transform ReferenceScaleTransform => useThisScale ? transform : referenceScaleTransform;
        [SerializeField][DrawIf(nameof(useThisScale), false)][DrawIf(nameof(gizmoType), GizmoType.WireMesh | GizmoType.Mesh)] private Transform referenceScaleTransform;
        #endregion fields & properties

        #region methods
        private void DrawWireSphere()
        {
            Gizmos.DrawWireSphere(ReferencePositionTransform.position, radius);
        }
        private void DrawWireCube()
        {
            Gizmos.DrawWireCube(ReferencePositionTransform.position, scale);
        }
        private void DrawWireMesh()
        {
            Gizmos.DrawWireMesh(mesh, ReferencePositionTransform.position, ReferenceRotationTransform.rotation, ReferenceScaleTransform.lossyScale);
        }
        private void DrawSphere()
        {
            Gizmos.DrawSphere(ReferencePositionTransform.position, radius);
        }
        private void DrawCube()
        {
            Gizmos.DrawCube(ReferencePositionTransform.position, scale);
        }
        private void DrawMesh()
        {
            Gizmos.DrawMesh(mesh, ReferencePositionTransform.position, ReferenceRotationTransform.rotation, ReferenceScaleTransform.lossyScale);
        }
        private void OnDrawGizmos()
        {
            if (!drawAlways) return;
            DrawChoosedGizmo();
        }
        private void OnDrawGizmosSelected()
        {
            if (drawAlways) return;
            DrawChoosedGizmo();
        }
        private void DrawChoosedGizmo()
        {
            if (!enabled) return;
            Gizmos.color = color;
            try
            {
                switch (gizmoType)
                {
                    case GizmoType.WireSphere: DrawWireSphere(); break;
                    case GizmoType.WireCube: DrawWireCube(); break;
                    case GizmoType.WireMesh: DrawWireMesh(); break;

                    case GizmoType.Sphere: DrawSphere(); break;
                    case GizmoType.Cube: DrawCube(); break;
                    case GizmoType.Mesh: DrawMesh(); break;
                }
            }
            catch { }
        }
        #endregion methods
        [System.Flags]
        public enum GizmoType
        {
            WireSphere = 1,
            WireCube = 2,
            WireMesh = 4,
            Sphere = 8,
            Cube = 16,
            Mesh = 32
        }
#endif //UNITY_EDITOR
    }
}
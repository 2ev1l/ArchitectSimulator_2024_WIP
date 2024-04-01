using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DebugStuff;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Game.DataBase
{
    public class ResourcePrefab : MonoBehaviour
    {
        #region fields & properties
        /// <summary>
        /// Unmarshalled gameObject
        /// </summary>
        public GameObject GameObject
        {
            get
            {
                if (_gameObject == null)
                    _gameObject = gameObject;
                return _gameObject;
            }
        }
        private GameObject _gameObject;
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
        public Renderer Render => render;
        [SerializeField] private Renderer render;
        /// <summary>
        /// Length x Height x Width (cm)
        /// </summary>
        public Vector3Int SizeCentimeters => sizeCentimeters;
        [SerializeField] private Vector3Int sizeCentimeters;
        /// <summary>
        /// Length x Height x Width (m)
        /// </summary>
        public Vector3 SizeMeters => Vector3.right * (SizeCentimeters.x / 100f) + Vector3.up * (SizeCentimeters.y / 100f) + Vector3.forward * (SizeCentimeters.z / 100f);

        /// <summary>
        /// Format [LxHxW]
        /// </summary>
        public string SizeText
        {
            get
            {
                if (sizeText == null)
                {
                    Vector3 size = SizeMeters;
                    sizeText = $"{size.x:F2}x{size.y:F2}x{size.z:F2}m";
                }

                return sizeText;
            }
        }
        [System.NonSerialized] private string sizeText = null;
        public float VolumeM3 => SizeMeters.x * SizeMeters.y * SizeMeters.z;
        //Why not in 'Info'? -Because this class requires work with renderer in scene/game view
        public IReadOnlyList<ResourceColorInfo> MaterialsInfo => materialsInfo;
        /// <summary>
        /// Returns copy of original list
        /// </summary>
        internal List<ResourceColorInfo> MaterialsInfoInternal => materialsInfo.ToList();
        [SerializeField] private List<ResourceColorInfo> materialsInfo = new();
        #endregion fields & properties

        #region methods
        public void ChangeColor(int newColorId)
        {
            render.materials = materialsInfo[newColorId].MaterialsInternal.ToArray();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (render == null)
            {
                if (!TryGetComponent(out render))
                {
                    transform.GetChild(0).TryGetComponent(out render);
                }
            }
            int materialsCount = materialsInfo.Count;
            for (int i = 0; i < materialsCount; ++i)
            {
                materialsInfo[i].Id = i;
            }
        }
        [Button(nameof(SetApproximateSize))]
        private void SetApproximateSize()
        {
            Vector3 extents = render.bounds.extents;
            float scale = 20;
            Undo.RecordObject(gameObject, "Set size");
            sizeCentimeters = new Vector3Int(Mathf.CeilToInt(extents.x * scale) * 10, Mathf.CeilToInt(extents.y * scale) * 10, Mathf.CeilToInt(extents.z * scale) * 10);
        }
        [Button(nameof(AddCurrentMaterials))]
        private void AddCurrentMaterials()
        {
            Undo.RegisterCompleteObjectUndo(this, "Add color info");
            ResourceColorInfo colorInfo = new();
            colorInfo.MaterialsInternal.AddRange(render.sharedMaterials);

            int colorId = materialsInfo.Count;
            colorInfo.Color = (ResourceColor)colorId;
            materialsInfo.Add(colorInfo);
            EditorUtility.SetDirty(this);
        }
        [Button(nameof(DrawSizeGizmo))]
        private void DrawSizeGizmo()
        {
            GameObject newObject = new(SizeText);
            DebugStuff.DrawGizmo gizmoDrawer = newObject.AddComponent<DebugStuff.DrawGizmo>();
            gizmoDrawer.Gizmo = DebugStuff.DrawGizmo.GizmoType.WireCube;
            gizmoDrawer.Scale = SizeMeters;
            newObject.transform.position = render.bounds.center;
        }
#endif //UNITY_EDITOR
        #endregion methods
    }
}
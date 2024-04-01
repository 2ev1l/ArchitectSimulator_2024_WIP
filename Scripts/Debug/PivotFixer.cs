using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PlasticGui.LaunchDiffParameters;




#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace DebugStuff
{
    public class PivotFixer : MonoBehaviour
    {
#if UNITY_EDITOR
        #region fields & properties
        [SerializeField] private Transform customTransform;
        #endregion fields & properties

        #region methods
        [Button(nameof(ToRenderersCenter))]
        private void ToRenderersCenter() => SetPivotToChildsRenderer(transform);
        [Button(nameof(ToChildsCenter))]
        private void ToChildsCenter() => SetPivotToChildsCenter(transform);
        [Button(nameof(ToCustomTransform))]
        private void ToCustomTransform() => SetPivotTo(customTransform, transform);
        /// <summary>
        /// EDITOR USE ONLY
        /// </summary>
        public static void SetPivotToChildsRenderer(Transform objectTransform)
        {
            TransformChildPosition(x =>
            {
                if (x.TryGetComponent(out Renderer renderer))
                    return renderer.bounds.center;
                return x.transform.position;
            }, objectTransform);
        }
        /// <summary>
        /// EDITOR USE ONLY
        /// </summary>
        public static void SetPivotToChildsCenter(Transform objectTransform)
        {
            TransformChildPosition(x =>
            {
                return x.position;
            }, objectTransform);
        }
        /// <summary>
        /// EDITOR USE ONLY
        /// </summary>
        public static void SetPivotTo(Transform pivot, Transform objectTransform)
        {
            TransformChildPosition(x =>
            {
                return pivot.position;
            }, objectTransform);
        }
        private static void TransformChildPosition(Func<Transform, Vector3> childResult, Transform objectTransform)
        {
            Undo.RegisterCompleteObjectUndo(objectTransform.gameObject, "Pivot change");
            int childCount = objectTransform.childCount;
            Vector3 sum = Vector3.zero;
            List<Transform> childs = new();
            for (int i = 0; i < childCount; ++i)
            {
                Transform child = objectTransform.GetChild(i);
                childs.Add(child);
                sum += childResult.Invoke(child); ;
            }
            Vector3 avgPos = sum / (float)childCount;
            ChangePivotTo(avgPos, objectTransform, childs);
            EditorUtility.SetDirty(objectTransform.gameObject);
        }
        private static void ChangePivotTo(Vector3 position, Transform objectTransform, List<Transform> childs)
        {
            Vector3 currentPivotPosition = objectTransform.position;
            Vector3 changeDirection = currentPivotPosition - position;
            objectTransform.position -= changeDirection;
            int childCount = childs.Count;
            for (int i = 0; i < childCount; ++i)
            {
                childs[i].position += changeDirection;
            }
        }
        #endregion methods
#endif //UNITY_EDITOR
    }
}
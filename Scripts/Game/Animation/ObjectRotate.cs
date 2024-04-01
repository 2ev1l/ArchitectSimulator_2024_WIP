using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Universal.Time;

namespace Game.Animation
{
    public class ObjectRotate : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnRotateEnd;
        [SerializeField] private Transform rotatedObject;

        [SerializeField] private bool useLocalRotations = false;
        [SerializeField] private bool storeRotation = false;
        public IReadOnlyList<Transform> Rotations => rotations;
        [SerializeField][DrawIf(nameof(storeRotation), true, DisablingType.ReadOnly)] private List<Transform> rotations;
        [SerializeField] private QuaternionTimeChanger rotateChanger;
        public float RotateTime
        {
            get => rotateTime;
            set => rotateTime = value;
        }
        [SerializeField][Min(0)] private float rotateTime = 1f;
        [SerializeField][Min(0)] private int currentRotationId = 0;
        #endregion fields & properties

        #region methods
        public void RotateCycle()
        {
            StartCoroutine(DoRotateCycle());
        }
        private IEnumerator DoRotateCycle()
        {
            foreach (var el in rotations)
            {
                if (rotatedObject == null) yield break;
                RotateTo(el);
                yield return new WaitForSeconds(rotateTime);
            }
        }
        public void RotateTo(int rotationId)
        {
            currentRotationId = rotationId;
            RotateTo(rotations[rotationId]);
        }
        [SerializedMethod]
        public void RotateToNext()
        {
            currentRotationId++;
            currentRotationId %= rotations.Count;
            RotateTo(currentRotationId);
        }
        [SerializedMethod]
        public void RotateTo(Transform rotation)
        {
            TryStoreRotation(rotation);
            if (useLocalRotations)
                RotateTo(rotation.localRotation);
            else
                RotateTo(rotation.rotation);

        }
        public void RotateTo(Quaternion rotation)
        {
            if (useLocalRotations)
            {
                rotateChanger.SetValues(rotatedObject.localRotation, rotation);
                rotateChanger.SetActions(x => rotatedObject.localRotation = x, delegate { OnRotateEnd?.Invoke(); }, delegate { return rotatedObject == null; }, false);
            }
            else
            {
                rotateChanger.SetValues(rotatedObject.rotation, rotation);
                rotateChanger.SetActions(x => rotatedObject.rotation = x, delegate { OnRotateEnd?.Invoke(); }, delegate { return rotatedObject == null; }, false);
            }
            rotateChanger.Restart(rotateTime);
        }

        private void TryStoreRotation(Transform rotation)
        {
            if (!storeRotation) return;
            if (rotations.Contains(rotation)) return;
            rotations.Add(rotation);
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (drawAlways) return;
            DrawRotations();
        }
        private void OnDrawGizmos()
        {
            if (!drawAlways) return;
            DrawRotations();
        }
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool drawAlways = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private Mesh debugMesh;
        private void DrawRotations()
        {
            if (!doDebug) return;
            if (rotations.Count == 0 || rotatedObject == null || debugMesh == null) return;

            for (int i = 0; i < rotations.Count; ++i)
            {
                Transform rotation = rotations[i];
                Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)i / rotations.Count);
                Gizmos.DrawWireMesh(debugMesh, rotatedObject.position, useLocalRotations ? rotation.rotation : rotation.localRotation, rotatedObject.lossyScale);
            }
        }
        [Title("Tests")]
        [SerializeField][Min(0)] private int rotationToTest = 0;
        [Button(nameof(TestRotate))]
        private void TestRotate()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;

            RotateTo(rotationToTest);
        }
#endif //UNITY_EDITOR
    }
}
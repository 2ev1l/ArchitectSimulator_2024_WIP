using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Time;

namespace Game.Animation
{
    public class ObjectScale : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnScaledEvent;
        public UnityAction OnScaled;

        [SerializeField] private Transform scaleable;
        [SerializeField] private List<ScaleInfo> scales = new();
        public float ScaleTime => scaleTimeMultiplier;
        [SerializeField] private float scaleTimeMultiplier = 1f;
        [SerializeField] private VectorTimeChanger scaleChanger = new();
        private int currentScaleId = -1;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void ScaleToNext()
        {
            currentScaleId = (currentScaleId + 1) % scales.Count;
            ScaleTo(scales[currentScaleId]);
        }
        [SerializedMethod]
        public void ScaleTo(Vector3 scale) => ScaleTo(new ScaleInfo(scale, 1));
        [SerializedMethod]
        public void ScaleTo(ScaleInfo info)
        {
            scaleChanger.SetValues(scaleable.localScale, info.Scale);

            scaleChanger.SetActions(x => scaleable.localScale = x, delegate { scaleable.localScale = info.Scale; OnScaleEnd(); }, delegate { return scaleable == null; });
            scaleChanger.Restart(scaleTimeMultiplier * info.ScaleTime);
        }
        private void OnScaleEnd()
        {

        }

        #endregion methods
        [System.Serializable]
        public struct ScaleInfo
        {
            public readonly Vector3 Scale => scale;
            [SerializeField] private Vector3 scale;
            public readonly float ScaleTime => scaleTime;
            [SerializeField] private float scaleTime;

            public ScaleInfo(Vector3 scale, float scaleTime)
            {
                this.scale = scale;
                this.scaleTime = scaleTime;
            }
        }
    }
}
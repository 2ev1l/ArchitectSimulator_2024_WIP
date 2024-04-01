using UnityEngine;

namespace Game.Animation
{
    /// <summary>
    /// You need to invoke <see cref="TrySimulate"/> somewhere
    /// </summary>
    public abstract class RotateAroundBase : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Vector3 axis = Vector3.forward;
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool useLocalRotation = false;
        [SerializeField] private bool enableOnAwake = true;
        [SerializeField] private bool isPlaying = false;
        protected abstract float DeltaTime { get; }
        #endregion fields & properties

        #region methods
        public void StopPlaying() => isPlaying = false;
        public void StartPlaying() => isPlaying = true;
        private void Awake()
        {
            if (!enableOnAwake) return;
            StartPlaying();
        }
        protected virtual void TrySimulate()
        {
            if (!isPlaying) return;
            Simulate();
        }
        protected virtual void Simulate()
        {
            if (useLocalRotation)
            {
                transform.localRotation *= Quaternion.AngleAxis(speed * DeltaTime, axis);
            }
            else
            {
                transform.rotation *= Quaternion.AngleAxis(speed * DeltaTime, axis);
            }
        }
        #endregion methods
    }
}
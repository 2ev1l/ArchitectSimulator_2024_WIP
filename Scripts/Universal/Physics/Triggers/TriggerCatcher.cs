using UnityEngine;

namespace Universal.Physics.Triggers
{
    public class TriggerCatcher : CollisionTrigger
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        private void OnTriggerEnter(Collider other)
        {
            if (!IsTagExists(other, out _)) return;
            OnEnter?.Invoke();
            OnEnterEvent?.Invoke();
        }
        private void OnTriggerExit(Collider other)
        {
            if (!IsTagExists(other, out _)) return;
            OnExit?.Invoke();
            OnExitEvent?.Invoke();
        }
        #endregion methods
    }
}
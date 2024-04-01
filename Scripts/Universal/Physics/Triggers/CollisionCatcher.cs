using UnityEngine;

namespace Universal.Physics.Triggers
{
    public class CollisionCatcher : CollisionTrigger
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        private void OnCollisionEnter(Collision collision)
        {
            if (!IsTagExists(collision.collider, out _)) return;
            OnEnter?.Invoke();
            OnEnterEvent?.Invoke();
        }
        private void OnCollisionExit(Collision collision)
        {
            if (!IsTagExists(collision.collider, out _)) return;
            OnExit?.Invoke();
            OnExitEvent?.Invoke();
        }
        #endregion methods
    }
}
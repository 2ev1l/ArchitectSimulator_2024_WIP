using UnityEngine;

namespace Universal.Behaviour
{
    /// <summary>
    /// Required for script execution order.
    /// </summary>
    public abstract class SingleSceneInstanceBase : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected virtual void Awake()
        {
            TrySetInstance();
        }
        protected abstract void TrySetInstance();
        #endregion methods
    }
}
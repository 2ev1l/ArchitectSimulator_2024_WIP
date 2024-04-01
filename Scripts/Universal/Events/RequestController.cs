using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Core;

namespace Universal.Events
{
    public class RequestController : MonoBehaviour, IInitializable, IRequestExecutor
    {
        #region fields & properties
        public static RequestController Instance { get; private set; }
        private HashSet<IRequestExecutor> executors = new();
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        public void DisableExecution(IRequestExecutor obj) => executors.Remove(obj);
        public void EnableExecution(IRequestExecutor obj) => executors.Add(obj);

        public bool TryExecuteRequest(ExecutableRequest request)
        {
            bool executed = false;
            foreach (IRequestExecutor exec in executors)
            {
                if (exec.TryExecuteRequest(request))
                    executed = true;
            }
            return executed;
        }
        #endregion methods
    }
}
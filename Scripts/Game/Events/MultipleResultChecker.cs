using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Core;

namespace Game.Events
{
    public class MultipleResultChecker : ResultChecker
    {
        #region fields & properties
        [SerializeField] private List<ResultChecker> resultCheckers;
        [SerializeField] private ResultCombineOperator combineOperator = ResultCombineOperator.Or;
        #endregion fields & properties

        #region methods
        public override bool GetResult()
        {
            bool result = combineOperator.GetStartResult();
            int totalCount = resultCheckers.Count;
            for (int i = 0; i < totalCount; ++i)
            {
                result = combineOperator.Execute(result, resultCheckers[i].GetResult());
            }
            return result;
        }
        private void OnValidate()
        {
            if (resultCheckers.Exists(x => x == this, out ResultChecker exist))
            {
                Debug.LogError("This operation will crash the application. Fixing.");
                resultCheckers.Remove(exist);
            }
        }
        #endregion methods
    }
}
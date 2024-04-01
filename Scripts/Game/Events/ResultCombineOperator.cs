using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Events
{
    #region enum
    public enum ResultCombineOperator
    {
        And,
        Or
    }
    #endregion enum

    public static class ResultCombineOperatorExtension
    {
        #region methods
        public static bool Execute(this ResultCombineOperator op, bool firstValue, bool secondValue) => op switch
        {
            ResultCombineOperator.And => firstValue && secondValue,
            ResultCombineOperator.Or => firstValue || secondValue,
            _ => throw new System.NotImplementedException($"Combine operator: {op}"),
        };
        public static bool GetStartResult(this ResultCombineOperator op) => op switch
        {
            ResultCombineOperator.And => true,
            ResultCombineOperator.Or => false,
            _ => throw new System.NotImplementedException($"Combine operator: {op}"),
        };
        #endregion methods
    }
}
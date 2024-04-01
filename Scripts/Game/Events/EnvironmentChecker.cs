using EditorCustom.Attributes;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Events
{
    public class EnvironmentChecker : ResultChecker
    {
        #region fields & properties
        public EnvironmentData Context => GameData.Data.EnvironmentData;
        [SerializeField] private bool checkLastTransportUsed = true;
        [SerializeField][DrawIf(nameof(checkLastTransportUsed), true)] private TransportType reqiuredTransportType = TransportType.Unknown;
        [SerializeField] private ResultCombineOperator combineOperator = ResultCombineOperator.And;
        #endregion fields & properties

        #region methods
        public override bool GetResult()
        {
            bool result = combineOperator.GetStartResult();
            if (checkLastTransportUsed) result = combineOperator.Execute(result, reqiuredTransportType == Context.LastTransportUsed);

            return result;
        }
        #endregion methods
    }
}
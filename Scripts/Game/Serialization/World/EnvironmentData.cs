using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class EnvironmentData
    {
        #region fields & properties
        public UnityAction<TransportType> OnLastTransportUsedChanged;
        public TransportType LastTransportUsed
        {
            get => lastTransportUsed;
            set => SetLastTransportUsed(value);
        }
        [SerializeField] private TransportType lastTransportUsed = TransportType.Unknown;
        #endregion fields & properties

        #region methods
        private void SetLastTransportUsed(TransportType value)
        {
            lastTransportUsed = value;
            OnLastTransportUsedChanged?.Invoke(lastTransportUsed);
        }
        #endregion methods
    }
}
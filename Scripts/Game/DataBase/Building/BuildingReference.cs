using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public struct BuildingReference
    {
        #region fields & properties
        public readonly BlueprintExistType ExistType => existType;
        [SerializeField] private BlueprintExistType existType;
        public readonly int ExistReferenceId => existReferenceId;
        [SerializeField][Min(0)] private int existReferenceId;
        #endregion fields & properties

        #region methods
        public readonly bool SameAs(BuildingReference reference)
        {
            return (existType == reference.existType && reference.existReferenceId == existReferenceId);
        }
        public BuildingReference(BlueprintExistType existType, int existReferenceId)
        {
            this.existType = existType;
            this.existReferenceId = existReferenceId;
        }

        #endregion methods
    }
}
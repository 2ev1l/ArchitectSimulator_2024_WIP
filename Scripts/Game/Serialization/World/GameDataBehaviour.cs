using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Core;

namespace Game.Serialization.World
{
    internal sealed class GameDataBehaviour : MonoBehaviour, IInitializable
    {
        #region fields & properties
        [SerializeField] private GameData Context = null;
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Context = GameData.Data;
        }
        [Button(nameof(AddAllResourcesToWarehouse))]
        private void AddAllResourcesToWarehouse()
        {
            foreach (var el in DB.Instance.ConstructionResourceInfo.Data)
            {
                Context.CompanyData.WarehouseData.TryAddResource(el.Id, ResourceType.Construction, 1);
            }
        }
        #endregion methods
    }
}
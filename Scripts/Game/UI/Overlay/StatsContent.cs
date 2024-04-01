using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Overlay
{
    public abstract class StatsContent : MonoBehaviour
    {
        #region fields & properties
        protected static PlayerData PlayerData => GameData.Data.PlayerData;
        protected static CompanyData CompanyData => GameData.Data.CompanyData;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            UpdateUI();
        }
        protected virtual void OnDisable()
        {

        }
        /// <summary>
        /// In base, updates on enable
        /// </summary>
        public abstract void UpdateUI();
        #endregion methods
    }
}
using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Behaviour;

namespace Game.Environment.Observers
{
    public class TasksObserver : SingleSceneInstance<TasksObserver>
    {
        #region fields & properties
        private static TasksData Context => GameData.Data.PlayerData.Tasks;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            
        }
        private void OnDisable()
        {
            
        }

        #endregion methods
    }
}
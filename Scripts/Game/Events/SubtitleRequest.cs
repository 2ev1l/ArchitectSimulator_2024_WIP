using Game.DataBase;
using Game.Serialization.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.Events;

namespace Game.Events
{
    [System.Serializable]
    public class SubtitleRequest : ExecutableRequest
    {
        #region fields & properties
        public IEnumerable<int> Ids => ids;
        [SerializeField] private int[] ids;
        #endregion fields & properties

        #region methods
        public override void Close()
        {

        }

        public SubtitleRequest(params int[] ids)
        {
            this.ids = ids;
        }
        #endregion methods
    }
}
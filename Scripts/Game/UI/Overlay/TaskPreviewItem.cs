using Game.Serialization.World;
using Game.UI.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Universal.Collections.Generic;

namespace Game.UI.Overlay
{
    public class TaskPreviewItem : MonoBehaviour, IListUpdater<Sprite>
    {
        #region fields & properties
        [SerializeField] private Image preview;
        #endregion fields & properties

        #region methods
        public void OnListUpdate(Sprite param)
        {
            preview.sprite = param;
        }
        #endregion methods
    }
}
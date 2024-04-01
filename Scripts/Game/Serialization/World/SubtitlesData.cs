using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class SubtitlesData
    {
        #region fields & properties
        [SerializeField] private UniqueList<int> playedSubtitles = new();
        #endregion fields & properties

        #region methods
        public bool TryAddPlayedSubtitle(int id) => playedSubtitles.TryAddItem(id, x => x == id);
        public bool IsSubtitlePlayed(int id) => playedSubtitles.Exists(x => x == id, out _);
        #endregion methods
    }
}
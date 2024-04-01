using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class MoodInfo : DBInfo
    {
        #region fields & properties
        public Sprite Sprite => sprite;
        [SerializeField] private Sprite sprite;
        public int MinMood => minMood;
        [SerializeField][Range(0, 100)] private int minMood = 50;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
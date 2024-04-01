using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Collections.Generic;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class PlayerData
    {
        #region fields & properties
        public MonthData MonthData => monthData;
        [SerializeField] private MonthData monthData = new();
        public Wallet Wallet => wallet;
        [SerializeField] private Wallet wallet = new(1000);
        public RangedValue Mood => mood;
        [SerializeField] private RangedValue mood = new(100, new(0, 100));
        public TasksData Tasks => tasks;
        [SerializeField] private TasksData tasks = new(new UniqueList<TaskData>(new List<TaskData>() { new TaskData(0, 1, 1) }));
        public SubtitlesData SubtitlesData => subtitlesData;
        [SerializeField] private SubtitlesData subtitlesData = new();
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
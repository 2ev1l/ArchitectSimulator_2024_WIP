using System;
using UnityEngine;
namespace Universal.Time
{
    [System.Serializable]
    public class TimeAverage
    {
        #region fields & properties
        /// <summary>
        /// This parameter must be updated as soon as the original value changes. <br></br>
        /// !Don't use it every frame
        /// </summary>
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged();
            }
        }
        [SerializeField] private float value;

        [SerializeField][Min(0.01f)] private float valuesLifeTime = 5f;
        [SerializeField][Min(0f)] private float accuracyTime = 0.1f;
        [SerializeField] private TimeLiveableList<Tuple<float>> averageValues;
        private float lastTimeAdded = 0;
        private float TimeBetweenChanges => UnityEngine.Time.time - lastTimeAdded;
        private float waitedTime = 0f;
        #endregion fields & properties

        #region methods
        private void OnValueChanged()
        {
            waitedTime += TimeBetweenChanges;
            float currentTime = UnityEngine.Time.time;
            if (waitedTime > valuesLifeTime)
            {
                int maxDecreaseTimes = Mathf.FloorToInt(waitedTime / valuesLifeTime);
                float timeSpend = valuesLifeTime * maxDecreaseTimes;
                waitedTime -= timeSpend;
                averageValues.DecreaseListTime(currentTime - valuesLifeTime);
            }

            if (TimeBetweenChanges < accuracyTime) return;
            var timeLiveable = averageValues.StackObject(new(value), currentTime);
            timeLiveable.SetBaseTimeLife(currentTime);
            lastTimeAdded = currentTime;
        }
        /// <summary>
        /// When it calls immediately after <see cref="Value"/> is changed, there might be incorrect values. <br></br>
        /// The most correct values can be obtained after a few seconds when value is changed. <br></br>
        /// Try to increase/decrease <see cref="valuesLifeTime"/> for better results.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public float GetAverageSpeed(float currentValue)
        {
            int count = 0;
            float finalValue = 0f;
            float currentTime = UnityEngine.Time.time;
            foreach (var el in averageValues.TimeLiveables)
            {
                count++;
                float timeSpent = currentTime - el.BaseTimeLife;
                float weight = 1f / timeSpent;
                finalValue += Mathf.Lerp(currentValue, el.Object.Item1, weight);
            }
            float avgFinalValue = finalValue / count;
            float avg = count == 0 ? value : (avgFinalValue);
            float speed = currentValue - avg;
            return speed;
        }
        /// <summary>
        /// Set <see cref="Value"/> as original value updates and get average speed
        /// </summary>
        public void Init()
        {
            averageValues = new(0.01f, 0);
            lastTimeAdded = UnityEngine.Time.time;
        }
        /// <summary>
        /// Use <see cref="Init"/> to initialize
        /// </summary>
        public TimeAverage() { }
        #endregion methods
    }
}
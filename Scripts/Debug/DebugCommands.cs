using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DebugStuff
{
    [System.Serializable]
    public static class DebugCommands
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        /// <summary>
        /// Shows log if application not in play mode
        /// </summary>
        /// <returns></returns>
        public static bool IsApplicationPlaying()
        {
            if (!Application.isPlaying)
            {
                Log("Enter Play Mode First");
                return false;
            }
            return true;
        }
        public static void BenchmarkRepeat(int repeatTimes, System.Action action, string testName)
        {
            float startTime = Time.realtimeSinceStartup;
            for (int i = 0; i < repeatTimes; ++i)
            {
                action.Invoke();
            }
            float endTime = Time.realtimeSinceStartup;
            Log($"{testName} Test Success. \nAverage time for iteration = {(endTime - startTime) / (float)repeatTimes}");
        }
        private static void Log(string message)
        {
            Debug.Log(message);
        }
        /// <summary>
        /// Debug interfaces on validate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="errorMessage"></param>
        public static void CastInterfacesList<T>(IEnumerable<Component> list, string listName, Object sender)
        {
            int counter = 0;
            if (list.Count() == 0) return;
            try
            {
                foreach (var el in list.Cast<T>())
                {
                    counter++;
                }
            }
            catch { Debug.LogError($"[{listName}]: Item #{counter} is not in type of {typeof(T)}", sender); }
        }
        #endregion methods
    }
}
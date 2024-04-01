using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public abstract class DBScriptableObjectBase : ScriptableObject
    {
        #region fields & properties
        public abstract int Id { get; }
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Creates much garbage
        /// </summary>
        /// <returns></returns>
        protected int GetObjectId()
        {
            string name = base.name;
            int numberStart = 1 + name.LastIndexOf(" ");
            int numberEnd = name.Length - 1;
            int count = numberEnd - numberStart + 1;
            string subString = name.Substring(numberStart, count);
            int number = System.Convert.ToInt32(subString);
            return number;
        }
        #endregion methods
    }
}
using UnityEngine;

namespace Universal.Time
{
    [System.Serializable]
    public class ColorTimeChanger : BaseTimeChanger<Color>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void LerpValue(float lerp)
        {
            outValue = Color.Lerp(StartValue, FinalValue, lerp);
        }
        #endregion methods
    }
}
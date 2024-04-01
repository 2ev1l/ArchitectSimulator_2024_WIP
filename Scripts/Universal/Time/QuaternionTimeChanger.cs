using UnityEngine;

namespace Universal.Time
{
    [System.Serializable]
    public class QuaternionTimeChanger : BaseTimeChanger<Quaternion>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void LerpValue(float lerp)
        {
            outValue = Quaternion.Lerp(StartValue, FinalValue, lerp);
        }
        #endregion methods
    }
}
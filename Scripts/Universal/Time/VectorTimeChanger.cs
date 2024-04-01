using UnityEngine;

namespace Universal.Time
{
    [System.Serializable]
    public class VectorTimeChanger : BaseTimeChanger<Vector3>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void LerpValue(float lerp)
        {
            outValue = Vector3.Lerp(StartValue, FinalValue, lerp);
        }
        #endregion methods
    }
}
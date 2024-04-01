using EditorCustom.Attributes;
using Universal.Events;

namespace Game.Animation
{
    /// <summary>
    /// Using fixed update for physics callbacks
    /// </summary>
    public class RotateAroundFixed : RotateAroundBase, IFixedUpdateSender
    {
        #region fields & properties
        protected override float DeltaTime => UnityEngine.Time.fixedDeltaTime;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            MessageController.Instance.AddSender(this);
        }
        private void OnDisable()
        {
            MessageController.Instance.RemoveSender(this);
        }
        public void FixedUpdateMessage()
        {
            TrySimulate();
        }
        [Button(nameof(Simulate))]
        protected override void Simulate()
        {
            base.Simulate();
        }
        #endregion methods
    }
}
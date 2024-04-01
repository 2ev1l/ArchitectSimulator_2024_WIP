using EditorCustom.Attributes;
using Game.Events;
using Game.Serialization.Settings.Input;
using Game.Serialization.World;
using UnityEngine;
using Universal.Behaviour;

namespace Game.Player
{
    public class Input : SingleSceneInstance<Input>
    {
        #region fields & properties
        public Moving Moving => moving;
        [SerializeField] private Moving moving;
        public Interaction Interaction => interaction;
        [SerializeField] private Interaction interaction;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            InputController.OnKeyHold += CheckHoldKey;
            InputController.OnKeyDown += CheckDownKey;
        }
        private void OnDisable()
        {
            InputController.OnKeyHold -= CheckHoldKey;
            InputController.OnKeyDown -= CheckDownKey;
        }

        private void CheckDownKey(KeyCodeInfo info)
        {
            CheckDownKey(info.Description);
        }
        private void CheckHoldKey(KeyCodeInfo info)
        {
            CheckHoldKey(info.Description);
        }
        private void CheckHoldKey(KeyCodeDescription description)
        {
            switch (description)
            {
                case KeyCodeDescription.MoveForward: moving.AddInputMove(Vector2.right); break;
                case KeyCodeDescription.MoveBackward: moving.AddInputMove(-Vector2.right); break;
                case KeyCodeDescription.MoveRight: moving.AddInputMove(Vector2.up); break;
                case KeyCodeDescription.MoveLeft: moving.AddInputMove(-Vector2.up); break;
                case KeyCodeDescription.Run: moving.DoAccelerate = true; break;
                default: break;
            }
        }
        private void CheckDownKey(KeyCodeDescription description)
        {
            switch (description)
            {
                case KeyCodeDescription.Interact: interaction.TryInteract(); break;
                default: break;
            }
        }
        #endregion methods
    }
}
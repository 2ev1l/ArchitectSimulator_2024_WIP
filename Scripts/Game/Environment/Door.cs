using EditorCustom.Attributes;
using Game.Animation;
using Game.Audio;
using Game.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class Door : InteractableObject
    {
        #region fields & properties
        [Title("Door")]
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClipData openClip;
        [SerializeField] private AudioClipData closeClip;
        public bool IsOpened => isOpened;
        [SerializeField][ReadOnly] private bool isOpened = false;
        private static readonly string openStateName = "Door-Open";
        private static readonly string closeStateName = "Door-Close";
        #endregion fields & properties

        #region methods
        protected override void OnInteract()
        {
            base.OnInteract();
            if (isOpened)
                Close();
            else
                Open();
        }
        private void Open()
        {
            isOpened = true;
            float normalizedTime = CustomAnimation.GetNormalizedAnimatorTime(animator, 0);
            animator.Play(openStateName, 0, 1 - normalizedTime);
            openClip.Play();
        }
        private void Close()
        {
            isOpened = false;
            float normalizedTime = CustomAnimation.GetNormalizedAnimatorTime(animator, 0);
            animator.Play(closeStateName, 0, 1 - normalizedTime);
            closeClip.Play();
        }
        #endregion methods
    }
}
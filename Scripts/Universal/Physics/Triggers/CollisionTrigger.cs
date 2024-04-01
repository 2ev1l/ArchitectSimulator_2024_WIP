using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.Physics.Triggers
{
    public abstract class CollisionTrigger : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnEnterEvent;
        public UnityAction OnEnter;

        public UnityEvent OnExitEvent;
        public UnityAction OnExit;

        protected IReadOnlyList<string> AllowedTags => allowedTags;
        [SerializeField][TagSelector] private List<string> allowedTags = new() { "Player" };
        #endregion fields & properties

        #region methods
        protected bool IsTagExists(Collider other, out string tag)
        {
            tag = allowedTags.Find(x => other.CompareTag(x));
            return tag != null;
        }
        #endregion methods
    }
}
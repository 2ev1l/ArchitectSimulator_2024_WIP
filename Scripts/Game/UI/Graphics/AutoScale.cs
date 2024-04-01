using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Graphics
{
    [RequireComponent(typeof(Renderer))]
    public class AutoScale : MonoBehaviour
    {
        #region fields & properties
        [SerializeField][Min(0)] private Vector2 tileScale = Vector2.one;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            FixScale();
        }
        private void FixScale()
        {
            Renderer render = GetComponent<Renderer>();
            Vector2 newScale = transform.lossyScale * tileScale;
            foreach (Material mat in render.materials)
            {
                mat.mainTextureScale = newScale;
            }
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField][DontDraw] private bool ___debugBool;

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            FixScale();
        }
#endif //UNITY_EDITOR

    }
}
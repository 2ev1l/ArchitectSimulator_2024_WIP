using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class ResourceColorInfo
    {
        #region fields & properties
        /// <summary>
        /// Id of colors in list
        /// </summary>
        public int Id
        {
            get => id;
            internal set => id = value;
        }
        [SerializeField][Min(0)] private int id;
        public ResourceColor Color
        {
            get => color;
            internal set => color = value;
        }
        [SerializeField] private ResourceColor color = ResourceColor.White;
        public IReadOnlyList<Sprite> ColorPreview => colorPreview;
        internal List<Sprite> ColorPreviewInternal => colorPreview;
        [SerializeField] private List<Sprite> colorPreview = new();
        public IReadOnlyList<Material> Materials => materials;
        internal List<Material> MaterialsInternal => materials;
        [SerializeField] private List<Material> materials = new();
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}
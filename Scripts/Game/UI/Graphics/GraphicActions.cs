using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Universal.UI.Graphics
{
    public class GraphicActions : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Graphic graphic;
        [SerializeField] private List<Material> materials;
        [SerializeField] private List<Color> colors;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void ChangeMaterial(int index) => graphic.material = materials[index];
        [SerializedMethod]
        public void ChangeColor(int index) => graphic.color = colors[index];

        public void ChangeColorSettings(int index, Color newColor) => colors[index] = newColor;
        #endregion methods
    }
}
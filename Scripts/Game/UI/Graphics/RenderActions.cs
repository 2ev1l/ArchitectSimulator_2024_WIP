using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Universal.UI.Graphics
{
    public class RenderActions : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Renderer render;
        [SerializeField] private List<Material> materials;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void ChangeAllMaterials()
        {
            render.materials = materials.ToArray();
        }
        [SerializedMethod]
        public void AddMaterial(int index)
        {
            List<Material> materials = render.materials.ToList();
            materials.Add(this.materials[index]);
            render.materials = materials.ToArray();
        }
        [SerializedMethod]
        public void RemoveMaterialAt(int index)
        {
            List<Material> materials = render.materials.ToList();
            materials.RemoveAt(index);
            render.materials = materials.ToArray();
        }
        [SerializedMethod]
        public void RemoveLastMaterial() => RemoveMaterialAt(render.materials.Length - 1);
        private void OnValidate()
        {
            if (render == null)
                TryGetComponent(out render);
        }
        #endregion methods
    }
}
using EditorCustom.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Game.UI.Text;


#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Game.Events
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class InteractableObject : MonoBehaviour
    {
        #region fields & properties
        public static UnityAction<InteractableObject> OnCurrentSelectedChanged;
        public static InteractableObject CurrentSelected => currentSelected;
        private static InteractableObject currentSelected = null;

        public UnityEvent OnInteractedEvent;
        public UnityEvent OnSelectedEvent;
        public UnityEvent OnDeSelectedEvent;
        public UnityAction OnInteracted;
        public UnityAction OnSelected;
        public UnityAction OnDeSelected;

        [SerializeField][Required] private Material selectedMaterial;
        public Renderer Render => render;
        [SerializeField][Required] protected Renderer render;
        public LanguageInfo InteractDescription => interactDescription;
        [SerializeField] private LanguageInfo interactDescription = new(0, UI.Text.TextType.Game);

        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isSelected = false;
        [SerializeField][ReadOnly] private Material tempMaterial = null;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            DeSelect();
        }
        protected virtual void OnDisable()
        {
            DeSelect();
        }
        public void Select()
        {
            if (isSelected) return;
            isSelected = true;
            List<Material> materials = render.materials.ToList();
            materials.Add(selectedMaterial);
            render.materials = materials.ToArray();
            tempMaterial = render.materials[render.materials.Length - 1];
            currentSelected = this;
            OnCurrentSelectedChanged?.Invoke(currentSelected);
            OnSelected?.Invoke();
            OnSelectedEvent?.Invoke();
        }
        public void DeSelect()
        {
            if (!isSelected) return;
            isSelected = false;
            List<Material> materials = render.materials.ToList();
            materials.Remove(tempMaterial);
            render.materials = materials.ToArray();
            if (currentSelected == this)
                currentSelected = null;
            OnCurrentSelectedChanged?.Invoke(null);
            OnDeSelected?.Invoke();
            OnDeSelectedEvent?.Invoke();
        }

        public void Interact()
        {
            if (!isSelected) return;
            OnInteract();
            OnInteracted?.Invoke();
            OnInteractedEvent?.Invoke();
        }
        /// <summary>
        /// Invokes just before <see cref="OnInteracted"/>
        /// </summary>
        protected virtual void OnInteract() { }
        #endregion methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponent<Collider>().isTrigger)
                Debug.Log("Collider component must be not trigger");

            if (render == null)
                TryGetComponent(out render);
        }
#endif //UNITY_EDITOR
    }
}
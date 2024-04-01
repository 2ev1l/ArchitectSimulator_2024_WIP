using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Universal.Behaviour
{
    public class StaticPoolableObject : MonoBehaviour, IPoolableObject<StaticPoolableObject>
    {
        #region fields & properties
        /// <summary>
        /// Unmarshalled gameObject
        /// </summary>
        public GameObject GameObject
        {
            get
            {
                _gameObject ??= gameObject;
                return _gameObject;
            }
        }
        private GameObject _gameObject = null;
        public UnityAction<StaticPoolableObject> OnDestroyed { get; set; }
        public bool IsUsing { get; set; }
        #endregion fields & properties

        #region methods
        protected virtual void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
        public virtual void DisableObject()
        {
            if (GameObject.activeSelf)
                GameObject.SetActive(false);
            IsUsing = false;
        }
        protected virtual void Init()
        {

        }
        public virtual StaticPoolableObject InstantiateThis(Transform parentForSpawn)
        {
            StaticPoolableObject obj = Instantiate(this, parentForSpawn);
            return FakeInstantiateAgain(obj);
        }
        public virtual StaticPoolableObject FakeInstantiateAgain(StaticPoolableObject obj)
        {
            GameObject go = obj.GameObject;
            if (!go.activeSelf)
                go.SetActive(true);
            obj.Init();
            return obj;
        }
        #endregion methods
    }
}
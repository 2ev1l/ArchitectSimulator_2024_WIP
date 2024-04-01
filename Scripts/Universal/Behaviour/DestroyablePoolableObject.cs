using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Universal.Core;

namespace Universal.Behaviour
{
    public class DestroyablePoolableObject : MonoBehaviour, IPoolableObject<DestroyablePoolableObject>
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
        public UnityAction<DestroyablePoolableObject> OnDestroyed { get; set; }
        public bool IsUsing { get; set; }

        public float LiveTime
        {
            get => liveTime;
            protected set
            {
                liveTime = value;
                if (liveTime > destroyTime)
                    destroyTime = liveTime + 1;
                Init();
            }
        }
        [Title("Settings")]
        [SerializeField][Min(0)] private float liveTime = 3f;
        [SerializeField] private bool randomizeLiveTime = true;
        [SerializeField][DrawIf(nameof(randomizeLiveTime), true)][MinMaxSlider(0, 1)] private Vector2 randomLiveTimeScale;
        [SerializeField][Min(0)] private float destroyTime = 60f;
        #endregion fields & properties

        #region methods
        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
        private void DestroyObject()
        {
            DisableObject();
            Destroy(GameObject);
        }
        public void DisableObject()
        {
            if (GameObject.activeSelf)
                GameObject.SetActive(false);
            IsUsing = false;
            Invoke(nameof(DestroyObject), destroyTime);
        }
        private void Init()
        {
            CancelInvoke(nameof(DisableObject));
            CancelInvoke(nameof(DestroyObject));
            Invoke(nameof(DisableObject), randomizeLiveTime ? Random.Range(randomLiveTimeScale.x * liveTime, randomLiveTimeScale.y * liveTime) : liveTime);
        }
        public virtual DestroyablePoolableObject InstantiateThis(Transform parentForSpawn)
        {
            DestroyablePoolableObject obj = Instantiate(this, parentForSpawn);
            return FakeInstantiateAgain(obj);
        }
        public virtual DestroyablePoolableObject FakeInstantiateAgain(DestroyablePoolableObject obj)
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
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Universal.Core;
using Universal.Serialization;

namespace Universal.Behaviour
{
    public class SceneLoader : MonoBehaviour, IInitializable, IStartInitializable
    {
        #region fields & properties
        public static UnityAction OnSceneLoaded;
        /// <summary>
        /// <see cref="{T0}"/> loadingPercent;
        /// </summary>
        public static UnityAction<float> OnSceneLoading;
        /// <summary>
        /// <see cref="{T0}"/> oldSceneName;
        /// <see cref="{T1}"/> newSceneName;
        /// </summary>
        public static UnityAction<string, string> OnSceneChanged;
        /// <summary>
        /// <see cref="{T0}"/> offsetTime
        /// </summary>
        public static UnityAction<float> OnStartLoading;
        public static SceneLoader Instance { get; private set; }
        public static bool IsSceneLoading { get; private set; }
        public static float LoadingTime { get; private set; } = 0.5f;
        public static Scene CurrentScene => SceneManager.GetActiveScene();
        private static string SceneToLoad;
        private static bool DisableLoadingScreen = true;
        private static readonly string loadingSceneName = "Loading";
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        public void Start()
        {
            IsSceneLoading = false;
        }
        public void LoadScene(string scene, bool disableLoadingScreen = true) => LoadScene(scene, LoadingTime, disableLoadingScreen);
        public void LoadScene(string scene, float time, bool disableLoadingScreen = true)
        {
            OnStartLoading?.Invoke(time);
            IsSceneLoading = true;
            SceneToLoad = scene;
            DisableLoadingScreen = disableLoadingScreen;
            Invoke(nameof(SceneLoad), time);
            RemoveEvents();
        }
        private void SceneLoad() => StartCoroutine(LoadScene(SceneToLoad));
        private static IEnumerator LoadScene(string scene)
        {
            string oldScene = SceneManager.GetActiveScene().name;
            IsSceneLoading = true;
            if (SavingUtils.Instance.CanSave())
                SavingUtils.Instance.SaveGameData();

            if (!DisableLoadingScreen)
                yield return WaitLoadingScreen();

            AsyncOperation newSceneLoad = SceneManager.LoadSceneAsync(scene);
            while (!newSceneLoad.isDone)
            {
                OnSceneLoading?.Invoke(newSceneLoad.progress);
                yield return CustomMath.WaitAFrame();
            }

            IsSceneLoading = false;
            OnSceneLoaded?.Invoke();
            OnSceneChanged?.Invoke(oldScene, scene);
        }
        private static IEnumerator WaitLoadingScreen()
        {
            AsyncOperation loadingScene = SceneManager.LoadSceneAsync(loadingSceneName);
            yield return loadingScene;
            OnSceneLoaded?.Invoke();
            yield return new WaitForSeconds(0.5f);
            OnStartLoading?.Invoke(LoadingTime);
        }
        private static void RemoveEvents()
        {
            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem != null)
                eventSystem.SetActive(false);
        }
        #endregion methods
    }
}
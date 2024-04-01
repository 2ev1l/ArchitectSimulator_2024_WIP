using EditorCustom.Attributes;
using Game.DataBase;
using Game.Events;
using Game.Serialization;
using Game.UI.Text;
using UnityEngine;
using Universal.Behaviour;
using Universal.Events;
using Game.Player;

namespace Game.UI
{
    public class ButtonActions : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void LoadScene(string sceneName) => SceneLoader.Instance.LoadScene(sceneName);
        [SerializedMethod]
        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
        [SerializedMethod]
        public void SaveImmediate()
        {
            SavingController.Instance.SaveGameData();
        }
        [SerializedMethod]
        public void RequestExit()
        {
            ConfirmRequest cr = new(Exit, null, new LanguageInfo(4, TextType.Menu), new LanguageInfo(35, TextType.Menu));
            cr.Send();
        }
        private void Exit()
        {
            Application.Quit();
        }
        [SerializedMethod]
        public void RequestNewGameStart()
        {
            ConfirmRequest cr = new(StartNewGame, null, new LanguageInfo(1, TextType.Menu), new LanguageInfo(52, TextType.Menu));
            cr.Send();
        }
        private void StartNewGame()
        {
            SavingController.Instance.ResetTotalProgress();
            SceneLoader.Instance.LoadScene("Game");
        }
        [SerializedMethod]
        public void RequestMenuExit()
        {
            ConfirmRequest cr = new(ExitToMenu, null, new LanguageInfo(53, TextType.Menu), new LanguageInfo(59, TextType.Menu));
            cr.Send();
        }
        private void ExitToMenu()
        {
            SceneLoader.Instance.LoadScene("Menu");
        }
        #endregion methods
    }
}
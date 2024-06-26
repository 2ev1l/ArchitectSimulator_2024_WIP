using DebugStuff;
using EditorCustom.Attributes;
using UnityEngine;

namespace Game.Audio
{
    public class SoundPlayer : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private AudioClipData clipData;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void PlayClip() => clipData.Play();
        #endregion methods
#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private string ___testString;
        [Button(nameof(TestPlay))]
        private void TestPlay()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            PlayClip();
        }
#endif //UNITY_EDITOR
    }
}
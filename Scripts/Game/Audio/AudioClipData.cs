using EditorCustom.Attributes;
using UnityEngine;

namespace Game.Audio
{
    [System.Serializable]
    public class AudioClipData
    {
        #region fields & properties
        public AudioClip Clip
        {
            get => clip;
            set => clip = value;
        }
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool playAtRendererCenter = false;
        public Vector3 Position => playAtRendererCenter ? render.bounds.center : positionToPlay.position;
        [SerializeField][DrawIf(nameof(playAtRendererCenter), false)] private Transform positionToPlay;
        [SerializeField][DrawIf(nameof(playAtRendererCenter), true)] private Renderer render;
        [SerializeField] private AudioType audioType = AudioType.Sound;
        [SerializeField][Min(0)] private float soundScale = 1f;
        #endregion fields & properties

        #region methods
        public void Play()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif //UNITY_EDITOR
            AudioManager.NextSoundScale = soundScale;
            Vector3 playPosition = Vector3.zero;
            try { playPosition = Position; }
            catch
            {
                AudioManager.PlayClip(clip, audioType);
                return;
            }
            AudioManager.PlayClipAtPoint(clip, audioType, playPosition);
        }
        public AudioClipData() { }
        public AudioClipData(AudioClip clip, Transform positionToPlay)
        {
            this.clip = clip;
            this.positionToPlay = positionToPlay;
        }
        #endregion methods
    }
}
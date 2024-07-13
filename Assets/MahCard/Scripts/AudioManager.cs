using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource bgmSource;

        [SerializeField]
        private AudioSource sfxSource;

        public void PlayBGM(AudioClip clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}

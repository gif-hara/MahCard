using HK;
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

        public static void PlayBGM(AudioClip clip)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlayBGMInternal(clip);
        }

        public static void PlaySFX(AudioClip clip)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySFXInternal(clip);
        }

        private void PlayBGMInternal(AudioClip clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }

        private void PlaySFXInternal(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}

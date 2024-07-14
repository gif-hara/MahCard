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
            var instance = TinyServiceLocator.Resolve<AudioManager>();
            instance.bgmSource.clip = clip;
            instance.bgmSource.Play();
        }

        public static void PlaySFX(AudioClip clip)
        {
            var instance = TinyServiceLocator.Resolve<AudioManager>();
            instance.sfxSource.PlayOneShot(clip);
        }
    }
}

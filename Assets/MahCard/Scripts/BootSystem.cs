using Cysharp.Threading.Tasks;
using MahCard;
using UnityEngine;

namespace HK
{
    /// <summary>
    /// ブートシステム
    /// </summary>
    public sealed class BootSystem
    {
        /// <summary>
        /// ブートシステムが初期化完了したか返す
        /// </summary>
        public static UniTask IsReady
        {
            get
            {
                return UniTask.WaitUntil(() => initializeState == InitializeState.Initialized);
            }
        }

        /// <summary>
        /// 初期化の状態
        /// </summary>
        private enum InitializeState
        {
            Initializing,
            Initialized,
        }

        private static InitializeState initializeState = InitializeState.Initializing;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            InitializeInternalAsync().Forget();
        }

        private static async UniTask InitializeInternalAsync()
        {
            initializeState = InitializeState.Initializing;
            await InitializeAudioManagerAsync();
            initializeState = InitializeState.Initialized;
        }

        private static async UniTask InitializeAudioManagerAsync()
        {
            var audioManagerPrefab = await AssetLoader.LoadAsync<AudioManager>("System.AudioManager");
            var audioManager = Object.Instantiate(audioManagerPrefab);
            Object.DontDestroyOnLoad(audioManager.gameObject);
            TinyServiceLocator.Register(audioManager);
        }
    }
}
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public static class AssetLoader
    {
        public static async UniTask<T> LoadAsync<T>(string path) where T : Object
        {
            var result = await Resources.LoadAsync<T>(path).ToUniTask();
            return result as T;
        }
    }
}

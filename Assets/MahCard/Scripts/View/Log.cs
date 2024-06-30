using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Log : View
    {
        public override UniTask OnGameStartAsync(Game game)
        {
            Debug.Log("GameStart");
            return UniTask.CompletedTask;
        }

        public override UniTask OnDeckShuffledAsync(Game game)
        {
            Debug.Log("DeckShuffled");
            return UniTask.CompletedTask;
        }
    }
}

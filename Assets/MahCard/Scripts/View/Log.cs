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

        public override UniTask OnDrawCardAsync(Game game, User user, Card card)
        {
            Debug.Log($"{user.Name} Draw {card}");
            return UniTask.CompletedTask;
        }

        public override UniTask OnDecidedParentAsync(Game game, User user)
        {
            Debug.Log($"{user.Name} Decided Parent");
            return UniTask.CompletedTask;
        }

        public override UniTask OnStartTurnAsync(Game game, User user)
        {
            Debug.Log($"{user.Name} Start Turn");
            return UniTask.CompletedTask;
        }

        public override UniTask OnWinAsync(Game game, User user)
        {
            Debug.Log($"{user.Name} Win");
            return UniTask.CompletedTask;
        }
    }
}

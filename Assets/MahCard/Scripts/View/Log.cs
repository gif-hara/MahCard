using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class Log : View
    {
        public override UniTask OnBeginGameAsync(Game game, CancellationToken scope)
        {
            Debug.Log("GameStart");
            return UniTask.CompletedTask;
        }

        public override UniTask OnDeckShuffledAsync(Game game, CancellationToken scope)
        {
            Debug.Log("DeckShuffled");
            return UniTask.CompletedTask;
        }

        public override UniTask OnDrawCardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            Debug.Log($"{user.Name} Draw {card}");
            return UniTask.CompletedTask;
        }

        public override UniTask OnDecidedParentAsync(Game game, User user, CancellationToken scope)
        {
            Debug.Log($"{user.Name} Decided Parent");
            return UniTask.CompletedTask;
        }

        public override UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope)
        {
            Debug.Log($"{user.Name} Start Turn");
            return UniTask.CompletedTask;
        }

        public override UniTask OnWinAsync(Game game, User user, CancellationToken scope)
        {
            Debug.Log($"{user.Name} Win");
            return UniTask.CompletedTask;
        }

        public override UniTask OnDiscardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            Debug.Log($"{user.Name} Discard {card}");
            return UniTask.CompletedTask;
        }
        
        public override UniTask OnFilledDeckAsync(Game game, CancellationToken scope)
        {
            Debug.Log("Filled Deck");
            return UniTask.CompletedTask;
        }

        public override UniTask OnInvokeAbilityAsync(Game game, User user, Define.CardAbility ability, CancellationToken scope)
        {
            Debug.Log($"{user.Name} Invoke {ability}");
            return UniTask.CompletedTask;
        }
    }
}

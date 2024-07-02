using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Random : IAI
    {
        public async UniTask<int> DiscardAsync(User user, CancellationToken scope)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: scope);
            return UnityEngine.Random.Range(0, user.Cards.Count);
        }

        public UniTask<Define.DeckType> ChoiceDeckTypeAsync(Game game, User user, CancellationToken scope)
        {
            if (game.DiscardDeck.Count <= 0)
            {
                return UniTask.FromResult(Define.DeckType.Deck);
            }
            return UniTask.FromResult(UnityEngine.Random.Range(0, 2) == 0 ? Define.DeckType.Deck : Define.DeckType.DiscardDeck);
        }

        public UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }
    }
}

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

        public UniTask<Define.DeckType> ChoiceDeckTypeAsync(User user, CancellationToken scope)
        {
            return UniTask.FromResult(UnityEngine.Random.Range(0, 2) == 0 ? Define.DeckType.Deck : Define.DeckType.DiscardDeck);
        }
    }
}

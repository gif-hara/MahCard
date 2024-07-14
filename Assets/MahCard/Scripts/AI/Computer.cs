using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Computer : IAI
    {
        /// <summary>
        /// 正直者かどうか
        /// </summary>
        /// <remarks>
        /// <c>true</c>の場合は手札が揃うために努力する
        /// <c>false</c>の場合はランダムに手札を捨てる
        /// </remarks>
        private bool isHonesty = false;

        public UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope)
        {
            if (user.IsReadyHand(game.Rules))
            {
                isHonesty = true;
            }
            else
            {
                isHonesty = UnityEngine.Random.Range(0, 2) == 0;
            }
            isHonesty = true;
            return UniTask.CompletedTask;
        }

        public async UniTask<int> DiscardAsync(User user, CancellationToken scope)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: scope);
            if (isHonesty)
            {
                var discardCard = user.Cards
                    .GroupBy(c => c.Color)
                    .OrderBy(g => g.Count())
                    .First()
                    .First();
                return user.GetCardIndex(discardCard);
            }
            else
            {
                return UnityEngine.Random.Range(0, user.Cards.Count);
            }
        }

        public UniTask<Define.DeckType> ChoiceDeckTypeAsync(Game game, User user, CancellationToken scope)
        {
            if (game.DiscardDeck.Count <= 0)
            {
                return UniTask.FromResult(Define.DeckType.Deck);
            }
            return UniTask.FromResult(UnityEngine.Random.Range(0, 2) == 0 ? Define.DeckType.Deck : Define.DeckType.DiscardDeck);
        }
    }
}

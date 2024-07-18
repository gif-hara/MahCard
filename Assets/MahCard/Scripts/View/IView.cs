using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public interface IView
    {
        void Setup(Game game, CancellationToken scope);

        UniTask OnBeginGameAsync(Game game, CancellationToken scope);

        UniTask OnDeckShuffledAsync(Game game, CancellationToken scope);

        UniTask OnDrawCardAsync(Game game, User user, Card card, bool isFastDraw, CancellationToken scope);

        UniTask OnDecidedParentAsync(Game game, User user, CancellationToken scope);

        UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope);

        UniTask OnEndTurnAsync(Game game, User user, CancellationToken scope);

        UniTask OnWinAsync(Game game, User user, CancellationToken scope);

        UniTask OnSelectDiscardAsync(Game game, User user, CancellationToken scope);

        UniTask OnDiscardAsync(Game game, User user, Card card, bool isFastDraw, CancellationToken scope);

        UniTask OnFilledDeckAsync(Game game, CancellationToken scope);

        UniTask OnInvokeAbilityAsync(Game game, User user, Define.CardAbility ability, CancellationToken scope);

        UniTask OnGameWinAsync(Game game, User user, CancellationToken scope);
    }
}

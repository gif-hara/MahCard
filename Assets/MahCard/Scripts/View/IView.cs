using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public interface IView
    {
        void Setup(Game game);

        UniTask OnGameStartAsync(Game game, CancellationToken scope);

        UniTask OnDeckShuffledAsync(Game game, CancellationToken scope);

        UniTask OnDrawCardAsync(Game game, User user, Card card, CancellationToken scope);

        UniTask OnDecidedParentAsync(Game game, User user, CancellationToken scope);

        UniTask OnStartTurnAsync(Game game, User user, CancellationToken scope);

        UniTask OnWinAsync(Game game, User user, CancellationToken scope);

        UniTask OnDiscardAsync(Game game, User user, Card card, CancellationToken scope);
        
        UniTask OnFilledDeckAsync(Game game, CancellationToken scope);
    }
}

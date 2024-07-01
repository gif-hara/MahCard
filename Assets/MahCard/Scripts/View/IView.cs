using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public interface IView
    {
        void Setup(Game game);

        UniTask OnGameStartAsync(Game game);

        UniTask OnDeckShuffledAsync(Game game);

        UniTask OnDrawCardAsync(Game game, User user, Card card);

        UniTask OnDecidedParentAsync(Game game, User user);

        UniTask OnStartTurnAsync(Game game, User user);

        UniTask OnWinAsync(Game game, User user);

        UniTask OnDiscardAsync(Game game, User user, Card card);
        
        UniTask OnFilledDeckAsync(Game game);
    }
}

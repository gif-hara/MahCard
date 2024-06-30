using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class View : IView
    {
        public virtual UniTask OnGameStartAsync(Game game)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDeckShuffledAsync(Game game)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDrawCardAsync(Game game, User user, Card card)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDecidedParentAsync(Game game, User user)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnStartTurnAsync(Game game, User user)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnWinAsync(Game game, User user)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDiscardAsync(Game game, User user, Card card)
        {
            return UniTask.CompletedTask;
        }
    }
}

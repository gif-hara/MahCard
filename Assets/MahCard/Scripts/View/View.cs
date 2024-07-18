using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class View : IView
    {
        public virtual void Setup(Game game, CancellationToken scope)
        {
        }

        public virtual UniTask OnBeginGameAsync(Game game, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDeckShuffledAsync(Game game, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDrawCardAsync(Game game, User user, Card card, bool isFastDraw, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDecidedParentAsync(Game game, User user, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEndTurnAsync(Game game, User user, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnWinAsync(Game game, User user, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDiscardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnFilledDeckAsync(Game game, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnInvokeAbilityAsync(Game game, User user, Define.CardAbility ability, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnSelectDiscardAsync(Game game, User user, CancellationToken scope)
        {
            return UniTask.CompletedTask;
        }
    }
}

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
    }
}

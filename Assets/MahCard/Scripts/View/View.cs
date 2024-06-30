using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class View : IView
    {
        public virtual UniTask OnGameStartAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}

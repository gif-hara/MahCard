using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class View : IView
    {
        public UniTask OnGameStartAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}

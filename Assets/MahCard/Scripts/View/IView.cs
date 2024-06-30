using Cysharp.Threading.Tasks;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    public interface IView
    {
        UniTask OnGameStartAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}
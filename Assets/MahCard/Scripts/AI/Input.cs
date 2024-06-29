using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Input : IAI
    {
        public UniTask<int> DiscardAsync(CancellationToken scope)
        {
            return UniTask.FromResult(0);
        }
    }
}

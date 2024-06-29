using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Input : IAI
    {
        public UniTask<int> DiscardAsync(CancellationToken scope)
        {
            var source = new UniTaskCompletionSource<int>();
            return source.Task;
        }
    }
}

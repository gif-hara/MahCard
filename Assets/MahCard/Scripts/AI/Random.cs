using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Random : IAI
    {
        public UniTask<int> DiscardAsync(User user, CancellationToken scope)
        {
            var index = UnityEngine.Random.Range(0, user.Cards.Count);
            return UniTask.FromResult(index);
        }
    }
}

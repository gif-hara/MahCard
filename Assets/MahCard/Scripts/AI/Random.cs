using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Random : IAI
    {
        public async UniTask<int> DiscardAsync(User user, CancellationToken scope)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: scope);
            return UnityEngine.Random.Range(0, user.Cards.Count);
        }
    }
}

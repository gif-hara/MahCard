using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAI
    {
        UniTask BeginTurnAsync(CancellationToken scope);
    }
}

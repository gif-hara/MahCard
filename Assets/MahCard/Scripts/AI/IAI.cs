using System.Threading;
using Cysharp.Threading.Tasks;

namespace MahCard.AI
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAI
    {
        UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope);

        UniTask<int> DiscardAsync(User user, CancellationToken scope);

        UniTask<Define.DeckType> ChoiceDeckTypeAsync(Game game, User user, CancellationToken scope);
    }
}

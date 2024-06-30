using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
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
            Observable.EveryUpdate(scope)
                .Subscribe(_ =>
                {
                    if (Keyboard.current.digit1Key.wasPressedThisFrame)
                    {
                        source.TrySetResult(0);
                    }
                    else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                    {
                        source.TrySetResult(1);
                    }
                    else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                    {
                        source.TrySetResult(2);
                    }
                    else if (Keyboard.current.digit4Key.wasPressedThisFrame)
                    {
                        source.TrySetResult(3);
                    }
                    else if (Keyboard.current.digit5Key.wasPressedThisFrame)
                    {
                        source.TrySetResult(4);
                    }
                });
            return source.Task;
        }
    }
}
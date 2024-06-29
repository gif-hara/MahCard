using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TinyStateMachineSystems
{
    /// <summary>
    /// Represents a tiny state machine that allows changing states asynchronously.
    /// </summary>
    public sealed class TinyStateMachine : IDisposable
    {
        private CancellationTokenSource scope = null;

        private bool isDisposed = false;

        /// <summary>
        /// Event that is triggered when the state changes.
        /// </summary>
        public event Action<Func<CancellationToken, UniTask>> OnChangeState;

        ~TinyStateMachine()
        {
            Dispose();
        }

        /// <summary>
        /// Changes the state asynchronously.
        /// </summary>
        /// <param name="state">The state to change to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the state change.</param>
        /// <returns>A <see cref="UniTask"/> representing the asynchronous operation.</returns>
        public async UniTask ChangeAsync(Func<CancellationToken, UniTask> state, CancellationToken cancellationToken = default)
        {
            if (isDisposed)
            {
                return;
            }

            Clear();
            scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Wait for one frame to ensure that the previous state has completed before starting the next state.
            await UniTask.NextFrame(scope.Token);
            OnChangeState?.Invoke(state);
            await state(scope.Token);
        }

        /// <summary>
        /// Changes the state synchronously.
        /// </summary>
        /// <param name="state">The state to change to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the state change.</param>
        public void Change(Func<CancellationToken, UniTask> state, CancellationToken cancellationToken = default)
        {
            ChangeAsync(state, cancellationToken).Forget();
        }

        /// <summary>
        /// Disposes the state machine and cancels any ongoing state change.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            Clear();
            isDisposed = true;
        }

        /// <summary>
        /// Cancels any ongoing state change and clears the state machine.
        /// </summary>
        public void Clear()
        {
            scope?.Cancel();
            scope?.Dispose();
            scope = null;
        }
    }
}
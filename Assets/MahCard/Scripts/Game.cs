using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MahCard.View;
using TinyStateMachineSystems;
using UnityEngine;
using UnityEngine.Assertions;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Game
    {
        public List<User> Users { get; }

        public Deck Deck { get; }

        public Deck DiscardDeck { get; }

        public GameRules Rules { get; }

        public int SubjectIndex { get; }

        private readonly IView view;

        private readonly TinyStateMachine stateMachine = new();

        private UniTaskCompletionSource endGameCompletionSource = null;

        private Unity.Mathematics.Random random;

        private int parentIndex = 0;

        private int turnCount = 0;

        public Game(
            IEnumerable<User> users,
            GameRules rules,
            IView view,
            uint seed,
            int subjectIndex
            )
        {
            Users = new List<User>(users);
            Deck = rules.DeckBlueprint.CreateDeck();
            DiscardDeck = new Deck();
            Rules = rules;
            this.view = view;
            random = new Unity.Mathematics.Random(seed);
            SubjectIndex = subjectIndex;
        }

        public UniTask BeginAsync()
        {
            Assert.IsNull(endGameCompletionSource);
            endGameCompletionSource = new UniTaskCompletionSource();
            view.Setup(this);
            stateMachine.Change(StateGameStart);

            return endGameCompletionSource.Task;
        }

        public bool IsSubjectUser(User user)
        {
            return Users[SubjectIndex] == user;
        }

        private async UniTask StateGameStart(CancellationToken scope)
        {
            await view.OnGameStartAsync(this);
            Deck.Shuffle(random);
            await view.OnDeckShuffledAsync(this);
            foreach (var user in Users)
            {
                for (var i = 0; i < Rules.HandCardCount; i++)
                {
                    await DrawProcessAsync(user);
                }
            }
            parentIndex = random.NextInt(0, Users.Count);
            await view.OnDecidedParentAsync(this, Users[parentIndex]);
            stateMachine.Change(StateUserTurn);
        }

        private async UniTask StateUserTurn(CancellationToken scope)
        {
            var index = (parentIndex + turnCount) % Users.Count;
            var user = Users[index];
            await view.OnStartTurnAsync(this, user);
            await DrawProcessAsync(user);
            if (user.IsAllSame())
            {
                await view.OnWinAsync(this, user);
                endGameCompletionSource.TrySetResult();
                return;
            }
            var discardIndex = await user.AI.DiscardAsync(scope);
            var card = user.Discard(discardIndex);
            await view.OnDiscardAsync(this, user, card);
            DiscardDeck.Push(card);
            turnCount++;
            stateMachine.Change(StateUserTurn);
        }

        private async UniTask DrawProcessAsync(User user)
        {
            var card = user.Draw(Deck);
            await view.OnDrawCardAsync(this, user, card);
            if (Deck.IsEmpty())
            {
                Deck.Fill(DiscardDeck, random);
            }
        }
    }
}

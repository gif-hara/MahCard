using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        private readonly TinyStateMachine stateMachine = new();

        private UniTaskCompletionSource endGameCompletionSource = null;

        private Unity.Mathematics.Random random;

        private int parentIndex = 0;

        private int turnCount = 0;

        public Game(IEnumerable<User> users, Deck deck, Deck discardDeck, GameRules rules, uint seed)
        {
            random = new Unity.Mathematics.Random(seed);
            Users = new List<User>(users);
            Deck = deck;
            DiscardDeck = discardDeck;
            Rules = rules;
        }

        public UniTask BeginAsync()
        {
            Assert.IsNull(endGameCompletionSource);
            endGameCompletionSource = new UniTaskCompletionSource();
            stateMachine.Change(StateGameStart);

            return endGameCompletionSource.Task;
        }

        private UniTask StateGameStart(CancellationToken scope)
        {
            Debug.Log("GameStart");
            Deck.Shuffle(random);
            foreach (var user in Users)
            {
                for (var i = 0; i < Rules.HandCardCount; i++)
                {
                    user.Draw(Deck, DiscardDeck, random);
                }
                Debug.Log(user);
            }
            parentIndex = random.NextInt(0, Users.Count);
            Debug.Log($"ParentIndex: {parentIndex}");
            stateMachine.Change(StateUserTurn);
            return UniTask.CompletedTask;
        }

        private async UniTask StateUserTurn(CancellationToken scope)
        {
            var index = (parentIndex + turnCount) % Users.Count;
            var user = Users[index];
            user.Draw(Deck, DiscardDeck, random);
            Debug.Log($"UserTurn: {user}");
            if (user.IsAllSame())
            {
                Debug.Log($"{user.Name} Win!");
                endGameCompletionSource.TrySetResult();
                return;
            }
            var discardIndex = await user.AI.DiscardAsync(scope);
            var card = user.Discard(discardIndex);
            Debug.Log($"{user.Name} Discard: {card}");
            DiscardDeck.Push(card);
            turnCount++;
            stateMachine.Change(StateUserTurn);
        }
    }
}

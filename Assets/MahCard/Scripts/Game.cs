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

        public Game(IEnumerable<User> users, Deck deck, Deck discardDeck, GameRules rules)
        {
            Users = new List<User>(users);
            Deck = deck;
            DiscardDeck = discardDeck;
            Rules = rules;
        }

        public UniTask Begin()
        {
            Assert.IsNull(endGameCompletionSource);
            endGameCompletionSource = new UniTaskCompletionSource();
            stateMachine.Change(StateGameStart);

            return endGameCompletionSource.Task;
        }

        private UniTask StateGameStart(CancellationToken scope)
        {
            Debug.Log("GameStart");
            Deck.Shuffle();
            foreach (var user in Users)
            {
                for (var i = 0; i < Rules.HandCardCount; i++)
                {
                    user.Draw(Deck);
                }
                Debug.Log(user);
            }
            return UniTask.CompletedTask;
        }
    }
}

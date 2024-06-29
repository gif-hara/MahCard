using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinyStateMachineSystems;

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

        public Game(IEnumerable<User> users, Deck deck, Deck discardDeck, GameRules rules)
        {
            Users = new List<User>(users);
            Deck = deck;
            DiscardDeck = discardDeck;
            Rules = rules;
        }

        private UniTask StateGameStart(CancellationToken scope)
        {
            Deck.Shuffle();
            foreach (var user in Users)
            {
                for (var i = 0; i < Rules.HandCardCount; i++)
                {
                    user.Draw(Deck);
                }
            }
            return UniTask.CompletedTask;
        }
    }
}

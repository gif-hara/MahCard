using System.Collections.Generic;
using System.Linq;
using MahCard.AI;
using R3;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class User
    {
        public string Name { get; }

        public IAI AI { get; }

        public List<Card> Cards { get; }

        public Subject<int> OnSelectedCardIndex { get; } = new();

        public Subject<Define.DeckType> OnSelectedDeckType { get; } = new();

        /// <summary>
        /// リーチ状態かどうか
        /// </summary>
        public bool IsReadyHand(GameRules rules) => Cards.Count == rules.HandCardCount && Cards.All(c => c.Color == Cards[0].Color);

        public User(string name, IAI ai)
        {
            Name = name;
            AI = ai;
            Cards = new List<Card>();
        }

        public Card Draw(Deck deck, int offset = 0)
        {
            var result = deck.Draw(offset);
            Cards.Add(result);
            return result;
        }

        public Card Discard(int index)
        {
            var card = Cards[index];
            Cards.RemoveAt(index);
            return card;
        }

        public bool IsPossessionCard()
        {
            return Cards.Count > 0;
        }

        public bool IsWin(GameRules rules)
        {
            return Cards.Count == rules.HandCardCount + 1 && Cards.All(c => c.Color == Cards[0].Color);
        }

        public int GetCardIndex(Card card)
        {
            return Cards.FindIndex(c => c == card);
        }

        public override string ToString()
        {
            return $"{Name} ({string.Join(", ", Cards.Select(c => c.ToString()))})";
        }
    }
}

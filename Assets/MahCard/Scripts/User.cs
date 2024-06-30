using System.Collections.Generic;
using System.Linq;
using MahCard.AI;

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

        public User(string name, IAI ai)
        {
            Name = name;
            AI = ai;
            Cards = new List<Card>();
        }

        public Card Draw(Deck deck)
        {
            var result = deck.Draw();
            Cards.Add(result);
            return result;
        }

        public Card Discard(int index)
        {
            var card = Cards[index];
            Cards.RemoveAt(index);
            return card;
        }

        public bool IsAllSame()
        {
            return Cards.All(c => c.Color == Cards[0].Color);
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

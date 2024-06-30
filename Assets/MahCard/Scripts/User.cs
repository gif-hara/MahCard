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

        public void Draw(Deck deck)
        {
            Cards.Add(deck.Draw());
        }

        public Card Discard(int index)
        {
            var card = Cards[index];
            Cards.RemoveAt(index);
            return card;
        }

        public override string ToString()
        {
            return $"{Name} ({string.Join(", ", Cards.Select(c => c.ToString()))})";
        }
    }
}

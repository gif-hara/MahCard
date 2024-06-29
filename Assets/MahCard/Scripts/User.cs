using System.Collections.Generic;
using System.Linq;
using MahCard.AI;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct User
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

        public readonly void Draw(Deck deck)
        {
            Cards.Add(deck.Draw());
        }

        public readonly Card Discard(int index)
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

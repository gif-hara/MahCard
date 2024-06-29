using System.Collections.Generic;
using System.Linq;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct User
    {
        public string Name { get; }

        public List<Card> Cards { get; }

        public User(string name)
        {
            Name = name;
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

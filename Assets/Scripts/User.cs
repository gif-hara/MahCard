using System.Collections.Generic;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct User
    {
        public List<Card> Cards { get; }

        public User(IEnumerable<Card> cards)
        {
            Cards = new List<Card>(cards);
        }

        public readonly void Draw(Card card)
        {
            Cards.Add(card);
        }

        public readonly Card Discard(int index)
        {
            var card = Cards[index];
            Cards.RemoveAt(index);
            return card;
        }
    }
}

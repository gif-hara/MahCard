using System.Collections.Generic;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Deck
    {
        public Stack<Card> Cards { get; }

        public Deck(IEnumerable<Card> cards)
        {
            Cards = new Stack<Card>(cards);
        }

        public Card Draw()
        {
            return Cards.Pop();
        }

        public void Push(Card card)
        {
            Cards.Push(card);
        }
    }
}

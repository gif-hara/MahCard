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

        public void Shuffle(Unity.Mathematics.Random random)
        {
            var cards = Cards.ToArray();
            Cards.Clear();
            for (var i = cards.Length - 1; i > 0; i--)
            {
                var j = random.NextInt(0, i + 1);
                var temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
            foreach (var card in cards)
            {
                Cards.Push(card);
            }
        }
    }
}

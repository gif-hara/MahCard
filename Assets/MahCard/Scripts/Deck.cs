using System.Collections.Generic;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Deck
    {
        private readonly Stack<Card> cards;

        public int Count => cards.Count;

        public Deck()
        {
            cards = new Stack<Card>();
        }

        public Deck(IEnumerable<Card> cards)
        {
            this.cards = new Stack<Card>(cards);
        }

        public Card Draw()
        {
            return cards.Pop();
        }

        public void Push(Card card)
        {
            cards.Push(card);
        }

        public void Shuffle(Unity.Mathematics.Random random)
        {
            var cards = this.cards.ToArray();
            this.cards.Clear();
            for (var i = cards.Length - 1; i > 0; i--)
            {
                var j = random.NextInt(0, i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
            foreach (var card in cards)
            {
                this.cards.Push(card);
            }
        }

        public bool IsEmpty()
        {
            return cards.Count <= 0;
        }

        public void Fill(Deck other)
        {
            while (!other.IsEmpty())
            {
                cards.Push(other.Draw());
            }
        }

        public Card Peek()
        {
            return cards.Peek();
        }
    }
}

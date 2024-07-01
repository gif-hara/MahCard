using System.Collections.Generic;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Deck
    {
        public Stack<Card> Cards { get; }

        public Deck()
        {
            Cards = new Stack<Card>();
        }

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
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
            foreach (var card in cards)
            {
                Cards.Push(card);
            }
        }

        public bool IsEmpty()
        {
            return Cards.Count <= 0;
        }

        public void Fill(Deck other)
        {
            while (!other.IsEmpty())
            {
                Cards.Push(other.Draw());
            }
        }
    }
}

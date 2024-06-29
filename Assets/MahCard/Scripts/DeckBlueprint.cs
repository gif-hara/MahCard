using System.Collections.Generic;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "DeckBlueprint", menuName = "MahCard/DeckBlueprint")]
    public sealed class DeckBlueprint : ScriptableObject
    {
        [SerializeField]
        private List<CardBlueprint> cards;

        public Deck CreateDeck()
        {
            var deck = new Deck();
            foreach (var card in cards)
            {
                foreach (var c in card.CreateCards())
                {
                    deck.Push(c);
                }
            }
            return deck;
        }
    }
}

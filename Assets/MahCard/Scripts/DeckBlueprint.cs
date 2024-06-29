using System.Collections.Generic;
using System.Linq;
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
            return new Deck(cards.SelectMany(c => c.CreateCards()).ToList());
        }
    }
}

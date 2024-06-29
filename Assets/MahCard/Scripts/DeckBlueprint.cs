using System;
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
        private List<CardBlueprintBundle> cardBlueprints;
        public List<CardBlueprintBundle> CardBlueprints => cardBlueprints;

        [Serializable]
        public class CardBlueprintBundle
        {
            [SerializeField]
            private CardBlueprint cardBlueprint;
            public CardBlueprint CardBlueprint => cardBlueprint;

            [SerializeField]
            private int count;
            public int Count => count;
        }
    }
}

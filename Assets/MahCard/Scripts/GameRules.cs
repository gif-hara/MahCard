using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "GameRules", menuName = "MahCard/GameRules")]
    public sealed class GameRules : ScriptableObject
    {
        [SerializeField]
        private int handCardCount;
        public int HandCardCount => handCardCount;

        [SerializeField]
        private DeckBlueprint deckBlueprint;
        public DeckBlueprint DeckBlueprint => deckBlueprint;
    }
}

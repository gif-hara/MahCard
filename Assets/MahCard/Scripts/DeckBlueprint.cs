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
        public List<CardBlueprint> Cards => cards;
    }
}

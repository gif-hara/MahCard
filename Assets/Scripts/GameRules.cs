using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GameRules : ScriptableObject
    {
        [SerializeField]
        private int handCardCount;
        public int HandCardCount => handCardCount;
    }
}

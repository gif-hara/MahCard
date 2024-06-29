using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CardBlueprint : ScriptableObject
    {
        [SerializeField]
        private Define.CardColor color;
        public Define.CardColor Color => color;

        [SerializeField]
        private Define.CardAbility number;
        public Define.CardAbility Number => number;
    }
}

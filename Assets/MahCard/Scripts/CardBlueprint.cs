using System;
using System.Collections.Generic;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class CardBlueprint
    {
        [SerializeField]
        private Define.CardColor color;
        public Define.CardColor Color => color;

        [SerializeField]
        private Define.CardAbility number;
        public Define.CardAbility Number => number;

        [SerializeField]
        private int count;
        public int Count => count;

        public IEnumerable<Card> CreateCards()
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Card(color, number);
            }
        }
    }
}

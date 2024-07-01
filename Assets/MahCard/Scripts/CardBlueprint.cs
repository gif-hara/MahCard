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

        [SerializeField]
        private Define.CardAbility ability;

        [SerializeField]
        private int count;

        public IEnumerable<Card> CreateCards()
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Card(color, ability);
            }
        }
    }
}

using System;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class RoomData
    {
        [SerializeField]
        private GameRules gameRules;
        public GameRules GameRules => gameRules;

        [SerializeField]
        private int computerPlayerCount;
        public int ComputerPlayerCount => computerPlayerCount;
    }
}

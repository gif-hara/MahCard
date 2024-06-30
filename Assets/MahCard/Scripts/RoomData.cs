using System;
using MahCard.View;
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

        [SerializeReference, SubclassSelector]
        private IView view;
        public IView View => view;
    }
}

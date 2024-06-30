using System;
using System.Collections.Generic;
using HK;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GameSceneController : MonoBehaviour
    {
        [SerializeField]
        private RoomData debugRoomData;

        async void Start()
        {
            await BootSystem.IsReady;

            var users = new List<User>
            {
                new("Player", new AI.Input())
            };
            for (var i = 0; i < debugRoomData.ComputerPlayerCount; i++)
            {
                users.Add(new($"Computer {i + 1}", new AI.Input()));
            }
            var deck = debugRoomData.GameRules.DeckBlueprint.CreateDeck();
            var discardDeck = new Deck();
            var game = new Game(users, deck, discardDeck, debugRoomData.GameRules, (uint)DateTime.Now.Ticks);
            await game.BeginAsync();
            Debug.Log("GameEnd");
        }
    }
}

using System;
using System.Collections.Generic;
using HK;
using MahCard.View;
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
                users.Add(new($"Computer {i + 1}", new AI.Random()));
            }
            var game = new Game(
                users,
                debugRoomData.GameRules,
                debugRoomData.View,
                (uint)DateTime.Now.Ticks,
                0
                );
            await game.BeginAsync();
            Debug.Log("GameEnd");
        }
    }
}

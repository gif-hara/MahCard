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
            var users = new User[]
            {
                new("Player1"),
                new("Player2"),
                new("Player3"),
                new("Player4")
            };
            var deck = debugRoomData.GameRules.DeckBlueprint.CreateDeck();
            var discardDeck = new Deck();
            var game = new Game(users, deck, discardDeck, debugRoomData.GameRules);
            await game.Begin();
        }
    }
}

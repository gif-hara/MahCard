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
            Debug.Log("GameSceneController is ready!");
        }
    }
}

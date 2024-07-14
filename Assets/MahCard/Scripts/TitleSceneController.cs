using HK;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TitleSceneController : MonoBehaviour
    {
        [SerializeField]
        private RoomData debugRoomData;

        async void Start()
        {
            await BootSystem.IsReady;
        }
    }
}

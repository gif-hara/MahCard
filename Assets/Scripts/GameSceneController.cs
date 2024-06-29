using HK;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GameSceneController : MonoBehaviour
    {
        async void Start()
        {
            await BootSystem.IsReady;
            Debug.Log("GameSceneController is ready!");
        }
    }
}

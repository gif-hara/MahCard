using HK;
using UnityEngine;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TitleSceneController : MonoBehaviour
    {
        async void Start()
        {
            await BootSystem.IsReady;
        }
    }
}

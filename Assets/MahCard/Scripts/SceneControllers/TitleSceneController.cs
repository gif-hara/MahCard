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
        private HKUIDocument titleDocumentPrefab;

        [SerializeField]
        private HKUIDocument guideBookDocumentPrefab;

        async void Start()
        {
            await BootSystem.IsReady;
            var titleController = new UITitleController(titleDocumentPrefab, guideBookDocumentPrefab);
            titleController.Open(destroyCancellationToken);
        }
    }
}

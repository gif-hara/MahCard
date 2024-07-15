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

        [SerializeField]
        private GameRules gameRules;

        async void Start()
        {
            await BootSystem.IsReady;
            var titleController = new UITitleController(titleDocumentPrefab, guideBookDocumentPrefab, gameRules);
            titleController.Open(destroyCancellationToken);
        }
    }
}

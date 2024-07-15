using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UITitleController
    {
        private readonly HKUIDocument titleDocumentPrefab;

        private readonly HKUIDocument guideBookDocumentPrefab;

        public UITitleController(HKUIDocument titleDocumentPrefab, HKUIDocument guideBookDocumentPrefab)
        {
            this.titleDocumentPrefab = titleDocumentPrefab;
            this.guideBookDocumentPrefab = guideBookDocumentPrefab;
        }

        public void Open(CancellationToken scope)
        {
            var document = Object.Instantiate(this.titleDocumentPrefab);
            var anyClickAreaDocument = document.Q<HKUIDocument>("Area.AnyClick");
            var mainAreaDocument = document.Q<HKUIDocument>("Area.Main");
            anyClickAreaDocument.gameObject.SetActive(true);
            mainAreaDocument.gameObject.SetActive(false);

            anyClickAreaDocument.Q<Button>("Button").OnClickAsObservable()
                .Subscribe(_ =>
                {
                    mainAreaDocument.gameObject.SetActive(true);
                    anyClickAreaDocument.gameObject.SetActive(false);
                })
                .RegisterTo(document.destroyCancellationToken);

            mainAreaDocument.Q<Button>("Button.GameStart").OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneManager.LoadScene("Game");
                })
                .RegisterTo(document.destroyCancellationToken);

            mainAreaDocument.Q<Button>("Button.GuideBook").OnClickAsObservable()
                .Subscribe(_ =>
                {
                    var guideBookController = new UIGuideBookController(guideBookDocumentPrefab);
                    guideBookController.OpenAsync(scope).Forget();
                })
                .RegisterTo(document.destroyCancellationToken);
        }
    }
}
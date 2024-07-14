using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UIGuideBookController
    {
        private readonly HKUIDocument documentPrefab;

        public UIGuideBookController(HKUIDocument documentPrefab)
        {
            this.documentPrefab = documentPrefab;
        }

        public async UniTask OpenAsync(CancellationToken scope)
        {
            var document = Object.Instantiate(documentPrefab);
            var pageAreaDocument = document.Q<HKUIDocument>("Area.Page");
            var indexAreaDocument = document.Q<HKUIDocument>("Area.Index");
            var nextButton = indexAreaDocument.Q<Button>("Button.Next");
            var previousButton = indexAreaDocument.Q<Button>("Button.Previous");
            var indexText = indexAreaDocument.Q<TMP_Text>("Text");
            var pages = new List<GameObject>();
            var currentPageIndex = 0;
            while (true)
            {
                var page = pageAreaDocument.TryQ(currentPageIndex.ToString());
                if (page == null)
                {
                    break;
                }
                pages.Add(page);
                page.SetActive(false);
                currentPageIndex++;
            }
            currentPageIndex = 0;
            ChangePage(currentPageIndex);
            nextButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ChangePage(currentPageIndex + 1);
                })
                .RegisterTo(document.destroyCancellationToken);
            previousButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ChangePage(currentPageIndex - 1);
                })
                .RegisterTo(document.destroyCancellationToken);

            await document.Q<Button>("Button.Close").OnClickAsync(scope);

            Object.Destroy(document.gameObject);

            void ChangePage(int nextPageIndex)
            {
                pages[currentPageIndex].SetActive(false);
                pages[nextPageIndex].SetActive(true);
                currentPageIndex = nextPageIndex;
                nextButton.gameObject.SetActive(nextPageIndex < pages.Count - 1);
                previousButton.gameObject.SetActive(nextPageIndex > 0);
                indexText.text = $"{nextPageIndex + 1}/{pages.Count}";
            }
        }
    }
}

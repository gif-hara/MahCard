using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
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

        public async UniTask OpenAsync(CancellationTokenSource scope)
        {
            var document = Object.Instantiate(documentPrefab);

            await document.Q<Button>("Button.Close").OnClickAsync(scope.Token);

            Object.Destroy(document.gameObject);
        }
    }
}

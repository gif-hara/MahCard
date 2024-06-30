using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MahCard.View
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class DefaultGameView : View
    {
        [SerializeField]
        private HKUIDocument gameDocumentPrefab;

        private HKUIDocument gameDocument;

        private readonly Dictionary<Card, HKUIDocument> cardDocuments = new();

        public override void Setup(Game game)
        {
            gameDocument = UnityEngine.Object.Instantiate(gameDocumentPrefab);
        }

        public override UniTask OnDrawCardAsync(Game game, User user, Card card)
        {
            if (game.IsSubjectUser(user))
            {
                var cardPrefab = gameDocument.Q<HKUIDocument>("Prefab.UI.Card.Inside");
                var cardParent = gameDocument.Q<HKUIDocument>("SubjectArea").Q<RectTransform>("CardArea");
                var cardInstance = UnityEngine.Object.Instantiate(cardPrefab, cardParent);
                cardDocuments.Add(card, cardInstance);
                var mainImageKey = "MainImage";
                cardInstance.Q<Image>(mainImageKey).color = game.Rules.GetColor(card.Color);
                cardInstance.Q<Button>(mainImageKey).OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        user.OnSelectedCardIndex.OnNext(user.GetCardIndex(card));
                    })
                    .RegisterTo(cardInstance.destroyCancellationToken);
            }
            return UniTask.CompletedTask;
        }

        public override UniTask OnDiscardAsync(Game game, User user, Card card)
        {
            if (game.IsSubjectUser(user))
            {
                var cardInstance = cardDocuments[card];
                cardDocuments.Remove(card);
                UnityEngine.Object.Destroy(cardInstance.gameObject);
            }
            return UniTask.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using TMPro;
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
        
        private readonly Dictionary<User, HKUIDocument> userAreaDocuments = new();

        public override void Setup(Game game)
        {
            gameDocument = UnityEngine.Object.Instantiate(gameDocumentPrefab);
            for (var i = 0; i < game.Users.Count; i++)
            {
                var user = game.Users[i];
                if (game.IsMainUser(user))
                {
                    userAreaDocuments.Add(user, gameDocument.Q<HKUIDocument>("MainUserArea"));
                }
                else
                {
                    var subUserAreaPrefab = gameDocument.Q<HKUIDocument>("Prefab.UI.OtherUser");
                    var parent = gameDocument.Q<Transform>("OtherUserArea");
                    var subUserInstance = UnityEngine.Object.Instantiate(subUserAreaPrefab, parent);
                    userAreaDocuments.Add(user, subUserInstance);
                }
            }
        }

        public override async UniTask OnDrawCardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            var cardPrefab = gameDocument.Q<HKUIDocument>("Prefab.UI.Card");
            var cardParent = userAreaDocuments[user].Q<RectTransform>("CardArea");
            var cardInstance = UnityEngine.Object.Instantiate(cardPrefab, cardParent);
            cardDocuments.Add(card, cardInstance);
            const string mainImageKey = "MainImage";
            cardInstance.Q<Image>(mainImageKey).color = game.Rules.GetColor(card.Color);
            cardInstance.Q<TMP_Text>("AbilityText").SetText(card.Ability.ToString());
            if (game.IsMainUser(user))
            {
                cardInstance.Q<Button>(mainImageKey).OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        user.OnSelectedCardIndex.OnNext(user.GetCardIndex(card));
                    })
                    .RegisterTo(cardInstance.destroyCancellationToken);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: scope);
        }

        public override UniTask OnDiscardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            var cardInstance = cardDocuments[card];
            cardDocuments.Remove(card);
            UnityEngine.Object.Destroy(cardInstance.gameObject);
            return UniTask.CompletedTask;
        }
    }
}

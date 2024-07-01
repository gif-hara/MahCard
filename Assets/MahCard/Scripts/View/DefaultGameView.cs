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
        
        private HKUIDocument discardCardDocument;
        
        private int deckMaxCount;

        private readonly Dictionary<Card, HKUIDocument> cardDocuments = new();
        
        private readonly Dictionary<User, HKUIDocument> userAreaDocuments = new();

        public override void Setup(Game game)
        {
            gameDocument = UnityEngine.Object.Instantiate(gameDocumentPrefab);
            discardCardDocument = gameDocument.Q<HKUIDocument>("DiscardCard");
            deckMaxCount = game.Deck.Count;
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
            gameDocument.Q<TMP_Text>("DeckRemainingCount").SetText(game.Deck.Count.ToString());
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DeckArea"), game.Deck);
            var cardPrefab = gameDocument.Q<HKUIDocument>("Prefab.UI.Card");
            var cardParent = userAreaDocuments[user].Q<RectTransform>("CardArea");
            var cardInstance = UnityEngine.Object.Instantiate(cardPrefab, cardParent);
            cardDocuments.Add(card, cardInstance);
            Apply(cardInstance, game.Rules, card);
            SetCardPublicState(cardInstance, game.IsMainUser(user));
            if (game.IsMainUser(user))
            {
                cardInstance.Q<Button>("MainImage").OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        user.OnSelectedCardIndex.OnNext(user.GetCardIndex(card));
                    })
                    .RegisterTo(cardInstance.destroyCancellationToken);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: scope);
        }

        public override async UniTask OnDiscardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
            var cardInstance = cardDocuments[card];
            cardDocuments.Remove(card);
            UnityEngine.Object.Destroy(cardInstance.gameObject);
            discardCardDocument.gameObject.SetActive(true);
            SetCardPublicState(discardCardDocument, true);
            Apply(discardCardDocument, game.Rules, card);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: scope);
        }

        public override UniTask OnBeginGameAsync(Game game, CancellationToken scope)
        {
            discardCardDocument.gameObject.SetActive(false);
            return BeginNotification("Game Start!", 1.0f, scope);
        }

        public override UniTask OnWinAsync(Game game, User user, CancellationToken scope)
        {
            return BeginNotification($"{user.Name} Win!", 1.0f, scope);
        }

        public override UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope)
        {
            if (game.IsMainUser(user))
            {
                return BeginNotification($"{user.Name}'s Turn", 1.0f, scope);
            }
            return UniTask.CompletedTask;
        }

        public override UniTask OnInvokeAbilityAsync(Game game, User user, Define.CardAbility ability, CancellationToken scope)
        {
            return BeginNotification(ability.ToString(), 1.0f, scope);
        }

        public override UniTask OnFilledDeckAsync(Game game, CancellationToken scope)
        {
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DeckArea"), game.Deck);
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
            return UniTask.CompletedTask;
        }

        private async UniTask BeginNotification(string message, float waitSeconds, CancellationToken scope)
        {
            var notificationArea = gameDocument.Q("NotificationArea");
            gameDocument.Q<TMP_Text>("NotificationText").SetText(message);
            notificationArea.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: scope);
            notificationArea.SetActive(false);
        }

        private static void SetCardPublicState(HKUIDocument card, bool isPublic)
        {
            card.Q("PublicArea").SetActive(isPublic);
            card.Q("PrivateArea").SetActive(!isPublic);
        }
        
        private static void Apply(HKUIDocument cardDocument, GameRules rules, Card card)
        {
            cardDocument.Q<Image>("MainImage").color = rules.GetColor(card.Color);
            cardDocument.Q<TMP_Text>("AbilityText").SetText(card.Ability.ToString());
        }

        private void UpdateDeckView(HKUIDocument deckAreaDocument, Deck deck)
        {
            var deckAreaTransform = (RectTransform)deckAreaDocument.transform;
            var thicknessDocument = deckAreaDocument.Q<RectTransform>("Thickness");
            var rate = (float)deck.Count / deckMaxCount;
            var p = deckAreaTransform.anchoredPosition;
            deckAreaDocument.gameObject.SetActive(deck.Count > 0);
            deckAreaTransform.anchoredPosition = new Vector2(p.x, -deckMaxCount * (1 - rate));
            thicknessDocument.sizeDelta = new Vector2(thicknessDocument.sizeDelta.x, deckMaxCount * rate);
        }
    }
}

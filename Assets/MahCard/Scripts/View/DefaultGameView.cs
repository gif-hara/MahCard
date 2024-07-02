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

        [SerializeField]
        private bool isAlwaysHandVisible;

        private HKUIDocument gameDocument;

        private HKUIDocument discardCardDocument;

        private int deckMaxCount;

        private readonly Dictionary<Card, HKUIDocument> cardDocuments = new();

        private readonly Dictionary<User, HKUIDocument> userAreaDocuments = new();

        public override void Setup(Game game)
        {
            gameDocument = UnityEngine.Object.Instantiate(gameDocumentPrefab);
            discardCardDocument = gameDocument.Q<HKUIDocument>("DiscardCard");
            var deckCardDocument = gameDocument.Q<HKUIDocument>("DeckCard");
            SetCardPublicState(deckCardDocument, false);
            deckMaxCount = game.Deck.Count;
            foreach (var user in game.Users)
            {
                if (game.IsMainUser(user))
                {
                    userAreaDocuments.Add(user, gameDocument.Q<HKUIDocument>("MainUserArea"));
                }
                else
                {
                    var otherUserAreaPrefab = gameDocument.Q<HKUIDocument>("Prefab.UI.OtherUser");
                    var parent = gameDocument.Q<Transform>("OtherUserArea");
                    var otherUserDocument = UnityEngine.Object.Instantiate(otherUserAreaPrefab, parent);
                    userAreaDocuments.Add(user, otherUserDocument);
                }
            }
            var mainUser = game.GetMainUser();
            gameDocument.Q<Button>("DeckButton").OnClickAsObservable()
                .Subscribe(_ =>
                {
                    mainUser.OnSelectedDeckType.OnNext(Define.DeckType.Deck);
                })
                .RegisterTo(gameDocument.destroyCancellationToken);
            gameDocument.Q<Button>("DiscardDeckButton").OnClickAsObservable()
                .Subscribe(_ =>
                {
                    mainUser.OnSelectedDeckType.OnNext(Define.DeckType.DiscardDeck);
                })
                .RegisterTo(gameDocument.destroyCancellationToken);
        }

        public override async UniTask OnDrawCardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            gameDocument.Q<TMP_Text>("DeckRemainingCount").SetText(game.Deck.Count.ToString());
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DeckArea"), game.Deck);
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
            UpdateDiscardDeckView(game);
            var cardPrefab = gameDocument.Q<HKUIDocument>("Prefab.UI.Card");
            var cardParent = userAreaDocuments[user].Q<RectTransform>("CardArea");
            var cardDocument = UnityEngine.Object.Instantiate(cardPrefab, cardParent);
            cardDocuments.Add(card, cardDocument);
            Apply(cardDocument, card, game.Rules);
            SetCardPublicState(cardDocument, game.IsMainUser(user) || isAlwaysHandVisible);
            if (game.IsMainUser(user))
            {
                cardDocument.Q<Button>("Button").OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        user.OnSelectedCardIndex.OnNext(user.GetCardIndex(card));
                    })
                    .RegisterTo(cardDocument.destroyCancellationToken);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: scope);
        }

        public override async UniTask OnDiscardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
            var cardDocument = cardDocuments[card];
            cardDocuments.Remove(card);
            UnityEngine.Object.Destroy(cardDocument.gameObject);
            UpdateDiscardDeckView(game);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: scope);
        }

        public override UniTask OnBeginGameAsync(Game game, CancellationToken scope)
        {
            discardCardDocument.gameObject.SetActive(false);
            return BeginNotification("Game Start!", "", 1.0f, scope);
        }

        public override async UniTask OnWinAsync(Game game, User user, CancellationToken scope)
        {
            foreach (var card in user.Cards)
            {
                var cardDocument = cardDocuments[card];
                SetCardPublicState(cardDocument, true);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: scope);
            await BeginNotification($"{user.Name} Win!", "", 3.0f, scope);
        }

        public override UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope)
        {
            if (game.IsMainUser(user))
            {
                return BeginNotification($"{user.Name}'s Turn", "", 1.0f, scope);
            }
            return UniTask.CompletedTask;
        }

        public override UniTask OnInvokeAbilityAsync(Game game, User user, Define.CardAbility ability, CancellationToken scope)
        {
            return BeginNotification(ability.ToString(), GetAbilitySubMessage(ability), 3.0f, scope);
        }

        public override UniTask OnFilledDeckAsync(Game game, CancellationToken scope)
        {
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DeckArea"), game.Deck);
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
            return UniTask.CompletedTask;
        }

        private async UniTask BeginNotification(string mainMessage, string subMessage, float waitSeconds, CancellationToken scope)
        {
            var notificationArea = gameDocument.Q("NotificationArea");
            gameDocument.Q<TMP_Text>("NotificationMainText").SetText(mainMessage);
            var subText = gameDocument.Q<TMP_Text>("NotificationSubText");
            subText.SetText(subMessage);
            subText.gameObject.SetActive(subMessage.Length > 0);
            notificationArea.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: scope);
            notificationArea.SetActive(false);
        }

        private static void SetCardPublicState(HKUIDocument cardDocument, bool isPublic)
        {
            cardDocument.Q("PublicArea").SetActive(isPublic);
            cardDocument.Q("PrivateArea").SetActive(!isPublic);
        }

        private static void Apply(HKUIDocument cardDocument, Card card, GameRules rules)
        {
            cardDocument.Q<Image>("MainImage").color = rules.GetColor(card.Color);
            cardDocument.Q<TMP_Text>("AbilityText").SetText(card.Ability.ToString());
        }

        private void UpdateDeckView(HKUIDocument deckAreaDocument, Deck deck)
        {
            var deckAreaTransform = (RectTransform)deckAreaDocument.transform;
            var thicknessTransform = deckAreaDocument.Q<RectTransform>("Thickness");
            var rate = (float)deck.Count / deckMaxCount;
            var v = deckAreaTransform.anchoredPosition;
            deckAreaDocument.gameObject.SetActive(deck.Count > 0);
            deckAreaTransform.anchoredPosition = new Vector2(v.x, -deckMaxCount * (1 - rate));
            v = thicknessTransform.sizeDelta;
            thicknessTransform.sizeDelta = new Vector2(v.x, deckMaxCount * rate);
        }

        private void UpdateDiscardDeckView(Game game)
        {
            if (game.DiscardDeck.Count > 0)
            {
                discardCardDocument.gameObject.SetActive(true);
                SetCardPublicState(discardCardDocument, true);
                Apply(discardCardDocument, game.DiscardDeck.Peek(), game.Rules);
            }
            else
            {
                discardCardDocument.gameObject.SetActive(false);
            }
        }

        private static string GetAbilitySubMessage(Define.CardAbility ability)
        {
            return ability switch
            {
                Define.CardAbility.Reset => "全ての手札を捨ててデッキから4枚カードを引きます",
                Define.CardAbility.Retry => "もう1度デッキからカードを引いて1枚捨てます",
                Define.CardAbility.Double => "2枚の手札を捨ててデッキから2枚引きます",
                Define.CardAbility.Trade => "1番上にある捨札を引いて1枚捨てます",
                _ => string.Empty
            };
        }
    }
}

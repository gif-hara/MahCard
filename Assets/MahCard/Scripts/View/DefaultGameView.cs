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

        private HKUIDocument supplementDescriptionAreaDocument;

        private HKUIDocument sequencesDocument;

        private int deckMaxCount;

        private readonly Dictionary<Card, HKUIDocument> cardDocuments = new();

        private readonly Dictionary<User, HKUIDocument> userAreaDocuments = new();

        public override void Setup(Game game, CancellationToken scope)
        {
            gameDocument = UnityEngine.Object.Instantiate(gameDocumentPrefab);
            discardCardDocument = gameDocument.Q<HKUIDocument>("DiscardCard");
            supplementDescriptionAreaDocument = gameDocument.Q<HKUIDocument>("SupplementDescriptionArea");
            sequencesDocument = gameDocument.Q<HKUIDocument>("Sequences");
            var deckCardDocument = gameDocument.Q<HKUIDocument>("DeckCard");
            SetCardPublicState(deckCardDocument, false);
            deckMaxCount = game.Deck.Count;
            foreach (var user in game.Users)
            {
                if (game.IsMainUser(user))
                {
                    userAreaDocuments.Add(user, gameDocument.Q<HKUIDocument>("MainUserArea"));
                    userAreaDocuments[user].Q<TMP_Text>("UserName").SetText(user.Name);
                }
                else
                {
                    var otherUserAreaPrefab = gameDocument.Q<HKUIDocument>("Prefab.UI.OtherUser");
                    var parent = gameDocument.Q<Transform>("OtherUserArea");
                    var otherUserDocument = UnityEngine.Object.Instantiate(otherUserAreaPrefab, parent);
                    userAreaDocuments.Add(user, otherUserDocument);
                    otherUserDocument.Q<TMP_Text>("UserName").SetText(user.Name);
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
            supplementDescriptionAreaDocument.gameObject.SetActive(false);
            sequencesDocument.Q<SequenceMonobehaviour>("SupplementDescriptionAnimation").PlayAsync(scope).Forget();
        }

        public override async UniTask OnDecidedParentAsync(Game game, User user, CancellationToken scope)
        {
            await BeginNotificationAsync(
                $"{user.Name}が親です",
                "",
                game.Rules.GetSfxClip("Notification.Default"),
                scope
                );
        }

        public override async UniTask OnDrawCardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            gameDocument.Q<TMP_Text>("DeckRemainingCount").SetText(game.Deck.Count.ToString());
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DeckArea"), game.Deck);
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
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
            AudioManager.PlaySFX(game.Rules.GetSfxClip("DrawCard"));

            await cardDocument
                .Q<HKUIDocument>("Sequences")
                .Q<SequenceMonobehaviour>("DefaultInAnimation")
                .PlayAsync(scope);
        }

        public override UniTask OnSelectDiscardAsync(Game game, User user, CancellationToken scope)
        {
            if (game.IsMainUser(user))
            {
                SetSupplementDescription("手札をタップして捨てるカードを選択してください");
            }
            return UniTask.CompletedTask;
        }

        public override async UniTask OnDiscardAsync(Game game, User user, Card card, CancellationToken scope)
        {
            if (game.IsMainUser(user))
            {
                ClearSupplementDescription();
            }
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
            var cardDocument = cardDocuments[card];
            cardDocuments.Remove(card);
            UpdateDiscardDeckView(game);
            AudioManager.PlaySFX(game.Rules.GetSfxClip("DiscardCard"));
            await cardDocument
                .Q<HKUIDocument>("Sequences")
                .Q<SequenceMonobehaviour>("DefaultOutAnimation")
                .PlayAsync(scope);
            if (card.Ability != Define.CardAbility.None && game.CanInvokeAbility)
            {
                AudioManager.PlaySFX(game.Rules.GetSfxClip("InvokeAbility"));
                await PlayBrillianceEffectAnimationAsync(discardCardDocument, scope);
            }
            UnityEngine.Object.Destroy(cardDocument.gameObject);
        }

        public override UniTask OnBeginGameAsync(Game game, CancellationToken scope)
        {
            foreach (var document in userAreaDocuments.Values)
            {
                document.Q<Image>("Background").color = game.Rules.DefaultUserBackgroundColor;
                document.Q<TMP_Text>("UserName").color = game.Rules.DefaultUserNameColor;
            }
            discardCardDocument.gameObject.SetActive(false);
            return BeginNotificationAsync(
                "ゲーム開始！",
                $"同じ絵柄のカードを{game.Rules.HandCardCount + 1}枚揃えると勝利です！",
                game.Rules.GetSfxClip("Notification.Default"),
                scope
                );
        }

        public override async UniTask OnWinAsync(Game game, User user, CancellationToken scope)
        {
            AudioManager.PlaySFX(game.Rules.GetSfxClip("Win"));
            foreach (var card in user.Cards)
            {
                var cardDocument = cardDocuments[card];
                SetCardPublicState(cardDocument, true);
                PlayBrillianceEffectAnimationAsync(cardDocument, scope).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: scope);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: scope);
            await BeginNotificationAsync(
                $"{user.Name}の勝利！",
                "",
                game.Rules.GetSfxClip("Notification.Win"),
                scope
                );
        }

        public override async UniTask OnBeginTurnAsync(Game game, User user, CancellationToken scope)
        {
            userAreaDocuments[user].Q<Image>("Background").color = game.Rules.CurrentTurnUserBackgroundColor;
            userAreaDocuments[user].Q<TMP_Text>("UserName").color = game.Rules.CurrentTurnUserNameColor;
            if (game.IsMainUser(user))
            {
                await BeginNotificationAsync(
                    $"あなたのターンです",
                    "",
                    game.Rules.GetSfxClip("Notification.Default"),
                    scope
                    );
                SetSupplementDescription("デッキまたは捨札をタップしてカードを引いてください");
                user.OnSelectedDeckType
                    .Subscribe(_ =>
                    {
                        ClearSupplementDescription();
                    })
                    .RegisterTo(scope);
            }
        }

        public override UniTask OnEndTurnAsync(Game game, User user, CancellationToken scope)
        {
            userAreaDocuments[user].Q<Image>("Background").color = game.Rules.DefaultUserBackgroundColor;
            userAreaDocuments[user].Q<TMP_Text>("UserName").color = game.Rules.DefaultUserNameColor;
            return UniTask.CompletedTask;
        }

        public override UniTask OnInvokeAbilityAsync(Game game, User user, Define.CardAbility ability, CancellationToken scope)
        {
            return BeginNotificationAsync(
                ability.ToString(),
                GetAbilitySubMessage(ability, game.Rules),
                game.Rules.GetSfxClip("Notification.Default"),
                scope
                );
        }

        public override UniTask OnFilledDeckAsync(Game game, CancellationToken scope)
        {
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DeckArea"), game.Deck);
            UpdateDeckView(gameDocument.Q<HKUIDocument>("DiscardDeckArea"), game.DiscardDeck);
            return UniTask.CompletedTask;
        }

        private async UniTask BeginNotificationAsync(string mainMessage, string subMessage, AudioClip sfx, CancellationToken scope)
        {
            var scopeSource = CancellationTokenSource.CreateLinkedTokenSource(scope);
            var notificationDocument = gameDocument.Q<HKUIDocument>("NotificationArea");
            notificationDocument
                .Q<HKUIDocument>("MainTextArea")
                .Q<TMP_Text>("Text")
                .SetText(mainMessage);
            var subTextAreaDocument = notificationDocument.Q<HKUIDocument>("SubTextArea");
            subTextAreaDocument.Q<TMP_Text>("Text").SetText(subMessage);
            subTextAreaDocument.gameObject.SetActive(subMessage.Length > 0);
            notificationDocument.gameObject.SetActive(true);
            var sequencesDocument = gameDocument.Q<HKUIDocument>("Sequences");
            AudioManager.PlaySFX(sfx);
            await sequencesDocument.Q<SequenceMonobehaviour>("Notification.In.Animation").PlayAsync(scopeSource.Token);
            sequencesDocument.Q<SequenceMonobehaviour>("ClickHereAnimation").PlayAsync(scopeSource.Token).Forget();
            await notificationDocument.Q<Button>("Button").OnClickAsync(scopeSource.Token);
            await sequencesDocument.Q<SequenceMonobehaviour>("Notification.Out.Animation").PlayAsync(scopeSource.Token);
            notificationDocument.gameObject.SetActive(false);
            scopeSource.Cancel();
            scopeSource.Dispose();
        }

        private static void SetCardPublicState(HKUIDocument cardDocument, bool isPublic)
        {
            cardDocument.Q("PublicArea").SetActive(isPublic);
            cardDocument.Q("PrivateArea").SetActive(!isPublic);
        }

        private static void Apply(HKUIDocument cardDocument, Card card, GameRules rules)
        {
            var colorData = rules.GetColorData(card.Color);
            cardDocument.Q<Image>("MainImage").sprite = colorData.Sprite;
            cardDocument.Q<Image>("Background").color = colorData.Color;
            var abilityText = cardDocument.Q<TMP_Text>("AbilityText");
            abilityText.SetText(card.Ability.ToString());
            abilityText.gameObject.SetActive(card.Ability != Define.CardAbility.None);
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
            thicknessTransform.sizeDelta = new Vector2(v.x, deckMaxCount * rate + 27);
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

        private void SetSupplementDescription(string description)
        {
            supplementDescriptionAreaDocument.Q<TMP_Text>("Description").SetText(description);
            supplementDescriptionAreaDocument.gameObject.SetActive(true);
        }

        private void ClearSupplementDescription()
        {
            supplementDescriptionAreaDocument.gameObject.SetActive(false);
        }

        private UniTask PlayBrillianceEffectAnimationAsync(HKUIDocument cardDocument, CancellationToken scope)
        {
            return cardDocument
                .Q<HKUIDocument>("Sequences")
                .Q<SequenceMonobehaviour>("BrillianceEffectAnimation")
                .PlayAsync(scope);
        }

        private static string GetAbilitySubMessage(Define.CardAbility ability, GameRules rules)
        {
            return ability switch
            {
                Define.CardAbility.Reset => $"全ての手札を捨ててデッキから{rules.HandCardCount}枚カードを引きます",
                Define.CardAbility.Retry => "もう1度デッキからカードを引いて1枚捨てます",
                Define.CardAbility.Double => "2枚の手札を捨ててデッキから2枚引きます",
                Define.CardAbility.Trade => "1番上にある捨札を引いて1枚捨てます",
                _ => string.Empty
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MahCard.View;
using TinyStateMachineSystems;
using UnityEngine;
using UnityEngine.Assertions;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Game
    {
        public List<User> Users { get; }

        public Deck Deck { get; }

        public Deck DiscardDeck { get; }

        public GameRules Rules { get; }

        public int MainUserIndex { get; }

        private readonly IView view;

        private readonly TinyStateMachine stateMachine = new();

        private UniTaskCompletionSource endGameCompletionSource = null;

        private Unity.Mathematics.Random random;

        private int parentIndex = 0;

        private int turnCount = 0;

        private int CurrentUserIndex => (parentIndex + turnCount) % Users.Count;

        public Game(
            IEnumerable<User> users,
            RoomData roomData,
            uint seed,
            int mainUserIndex
            )
        {
            Users = new List<User>(users);
            Rules = roomData.GameRules;
            Deck = Rules.DeckBlueprint.CreateDeck();
            DiscardDeck = new Deck();
            this.view = roomData.View;
            random = new Unity.Mathematics.Random(seed);
            MainUserIndex = mainUserIndex;
        }

        public UniTask BeginAsync()
        {
            Assert.IsNull(endGameCompletionSource);
            endGameCompletionSource = new UniTaskCompletionSource();
            view.Setup(this);
            stateMachine.Change(StateBeginGame);

            return endGameCompletionSource.Task;
        }

        public bool IsMainUser(User user)
        {
            return Users[MainUserIndex] == user;
        }

        public User GetMainUser()
        {
            return Users[MainUserIndex];
        }

        private async UniTask StateBeginGame(CancellationToken scope)
        {
            await view.OnBeginGameAsync(this, scope);
            await DeckShuffleProcessAsync(Deck, scope);
            foreach (var user in Users)
            {
                for (var i = 0; i < Rules.HandCardCount; i++)
                {
                    await DrawProcessAsync(user, Deck, scope);
                }
            }
            parentIndex = random.NextInt(0, Users.Count);
            await view.OnDecidedParentAsync(this, Users[parentIndex], scope);
            stateMachine.Change(StateBeginTurn);
        }

        private async UniTask StateBeginTurn(CancellationToken scope)
        {
            var index = CurrentUserIndex;
            var user = Users[index];
            await user.AI.OnBeginTurnAsync(this, user, scope);
            await view.OnBeginTurnAsync(this, user, scope);
            var deckType = await user.AI.ChoiceDeckTypeAsync(this, user, scope);
            var isWin = await DrawProcessAsync(user, GetDeck(deckType), scope);
            if (isWin)
            {
                stateMachine.Change(StateEndGame);
                return;
            }
            var discardIndex = await user.AI.DiscardAsync(user, scope);
            var discardCard = await DiscardProcessAsync(user, discardIndex, scope);
            TryInvokeAbility(discardCard);
        }

        private async UniTask StateDiscardRetryCard(CancellationToken scope)
        {
            var user = Users[CurrentUserIndex];
            await view.OnInvokeAbilityAsync(this, user, Define.CardAbility.Retry, scope);
            var isWin = await DrawProcessAsync(user, Deck, scope);
            if (isWin)
            {
                stateMachine.Change(StateEndGame);
                return;
            }
            var discardIndex = await user.AI.DiscardAsync(user, scope);
            var discardCard = await DiscardProcessAsync(user, discardIndex, scope);
            TryInvokeAbility(discardCard);
        }

        private async UniTask StateDiscardResetCard(CancellationToken scope)
        {
            var user = Users[CurrentUserIndex];
            await view.OnInvokeAbilityAsync(this, user, Define.CardAbility.Reset, scope);
            while (user.IsPossessionCard())
            {
                await DiscardProcessAsync(user, 0, scope);
            }
            for (var i = 0; i < Rules.HandCardCount; i++)
            {
                await DrawProcessAsync(user, Deck, scope);
            }
            stateMachine.Change(StateEndTurn);
        }

        private async UniTask StateDiscardTradeCard(CancellationToken scope)
        {
            if (!CanInvokeTradeAbility())
            {
                stateMachine.Change(StateEndTurn);
                return;
            }
            var user = Users[CurrentUserIndex];
            await view.OnInvokeAbilityAsync(this, user, Define.CardAbility.Trade, scope);
            // この段階ではTradeアビリティのカードが捨てられているので一度引いておく
            var tempCard = DiscardDeck.Draw();
            var drawCard = user.Draw(DiscardDeck);
            DiscardDeck.Push(tempCard);
            await view.OnDrawCardAsync(this, user, drawCard, scope);
            if (user.IsWin(Rules))
            {
                stateMachine.Change(StateEndGame);
                return;
            }
            var discardIndex = await user.AI.DiscardAsync(user, scope);
            await DiscardProcessAsync(user, discardIndex, scope);
            stateMachine.Change(StateEndTurn);
        }

        private async UniTask StateDiscardDoubleCard(CancellationToken scope)
        {
            var user = Users[CurrentUserIndex];
            await view.OnInvokeAbilityAsync(this, user, Define.CardAbility.Double, scope);
            for (var i = 0; i < 2; i++)
            {
                var discardIndex = await user.AI.DiscardAsync(user, scope);
                await DiscardProcessAsync(user, discardIndex, scope);
            }

            for (var i = 0; i < 2; i++)
            {
                await DrawProcessAsync(user, Deck, scope);
            }
            stateMachine.Change(StateEndTurn);
        }

        private UniTask StateEndTurn(CancellationToken scope)
        {
            turnCount++;
            stateMachine.Change(StateBeginTurn);
            return UniTask.CompletedTask;
        }

        private UniTask StateEndGame(CancellationToken scope)
        {
            endGameCompletionSource.TrySetResult();
            return UniTask.CompletedTask;
        }

        private async UniTask<bool> DrawProcessAsync(User user, Deck deck, CancellationToken scope)
        {
            if (deck == Deck && deck.IsEmpty())
            {
                await DeckFillProcessAsync(deck, DiscardDeck, scope);
                await DeckShuffleProcessAsync(deck, scope);
            }
            var card = user.Draw(deck);
            await view.OnDrawCardAsync(this, user, card, scope);
            if (user.IsWin(Rules))
            {
                await view.OnWinAsync(this, user, scope);
                return true;
            }

            return false;
        }

        private async UniTask<Card> DiscardProcessAsync(User user, int discardIndex, CancellationToken scope)
        {
            var card = user.Discard(discardIndex);
            DiscardDeck.Push(card);
            await view.OnDiscardAsync(this, user, card, scope);
            return card;
        }

        private async UniTask DeckFillProcessAsync(Deck deck, Deck targetDeck, CancellationToken scope)
        {
            deck.Fill(targetDeck);
            await view.OnFilledDeckAsync(this, scope);
        }

        private async UniTask DeckShuffleProcessAsync(Deck deck, CancellationToken scope)
        {
            deck.Shuffle(random);
            await view.OnDeckShuffledAsync(this, scope);
        }

        private void TryInvokeAbility(Card card)
        {
            switch (card.Ability)
            {
                case Define.CardAbility.None:
                    stateMachine.Change(StateEndTurn);
                    break;
                case Define.CardAbility.Retry:
                    stateMachine.Change(StateDiscardRetryCard);
                    break;
                case Define.CardAbility.Reset:
                    stateMachine.Change(StateDiscardResetCard);
                    break;
                case Define.CardAbility.Trade:
                    stateMachine.Change(StateDiscardTradeCard);
                    break;
                case Define.CardAbility.Double:
                    stateMachine.Change(StateDiscardDoubleCard);
                    break;
                default:
                    Assert.IsTrue(false, $"Invalid card ability: {card.Ability}");
                    break;
            }
        }

        private Deck GetDeck(Define.DeckType deckType)
        {
            return deckType switch
            {
                Define.DeckType.Deck => Deck,
                Define.DeckType.DiscardDeck => DiscardDeck,
                _ => throw new ArgumentOutOfRangeException(nameof(deckType), deckType, null),
            };
        }

        private bool CanInvokeTradeAbility()
        {
            return DiscardDeck.Count >= 2;
        }
    }
}

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
            GameRules rules,
            IView view,
            uint seed,
            int mainUserIndex
            )
        {
            Users = new List<User>(users);
            Deck = rules.DeckBlueprint.CreateDeck();
            DiscardDeck = new Deck();
            Rules = rules;
            this.view = view;
            random = new Unity.Mathematics.Random(seed);
            MainUserIndex = mainUserIndex;
        }

        public UniTask BeginAsync()
        {
            Assert.IsNull(endGameCompletionSource);
            endGameCompletionSource = new UniTaskCompletionSource();
            view.Setup(this);
            stateMachine.Change(StateGameStart);

            return endGameCompletionSource.Task;
        }

        public bool IsMainUser(User user)
        {
            return Users[MainUserIndex] == user;
        }

        private async UniTask StateGameStart(CancellationToken scope)
        {
            await view.OnGameStartAsync(this, scope);
            Deck.Shuffle(random);
            await view.OnDeckShuffledAsync(this, scope);
            foreach (var user in Users)
            {
                for (var i = 0; i < Rules.HandCardCount; i++)
                {
                    await DrawProcessAsync(user, scope);
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
            await view.OnStartTurnAsync(this, user, scope);
            var isWin = await DrawProcessAsync(user, scope);
            if (isWin)
            {
                stateMachine.Change(StateGameEnd);
                return;
            }
            var discardIndex = await user.AI.DiscardAsync(user, scope);
            var discardCard = await DiscardProcessAsync(user, discardIndex, scope);
            TryInvokeAbility(discardCard);
        }
        
        private async UniTask StateDiscardRetryCard(CancellationToken scope)
        {
            var index = CurrentUserIndex;
            var user = Users[index];
            var isWin = await DrawProcessAsync(user, scope);
            if (isWin)
            {
                stateMachine.Change(StateGameEnd);
                return;
            }
            var discardIndex = await user.AI.DiscardAsync(user, scope);
            var discardCard = await DiscardProcessAsync(user, discardIndex, scope);
            TryInvokeAbility(discardCard);
        }
        
        private UniTask StateEndTurn(CancellationToken scope)
        {
            turnCount++;
            stateMachine.Change(StateBeginTurn);
            return UniTask.CompletedTask;
        }
        
        private UniTask StateGameEnd(CancellationToken scope)
        {
            endGameCompletionSource.TrySetResult();
            return UniTask.CompletedTask;
        }

        private async UniTask<bool> DrawProcessAsync(User user, CancellationToken scope)
        {
            if (Deck.IsEmpty())
            {
                Deck.Fill(DiscardDeck);
                await view.OnFilledDeckAsync(this, scope);
                Deck.Shuffle(random);
            }
            var card = user.Draw(Deck);
            await view.OnDrawCardAsync(this, user, card, scope);
            if (user.IsAllSame())
            {
                await view.OnWinAsync(this, user, scope);
                return true;
            }

            return false;
        }

        private async UniTask<Card> DiscardProcessAsync(User user, int discardIndex, CancellationToken scope)
        {
            var card = user.Discard(discardIndex);
            await view.OnDiscardAsync(this, user, card, scope);
            DiscardDeck.Push(card);
            return card;
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
                default:
                    Assert.IsTrue(false, $"Invalid card ability: {card.Ability}");
                    break;
            }
        }
    }
}

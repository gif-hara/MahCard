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
            stateMachine.Change(StateBeginGame);

            return endGameCompletionSource.Task;
        }

        public bool IsMainUser(User user)
        {
            return Users[MainUserIndex] == user;
        }

        private async UniTask StateBeginGame(CancellationToken scope)
        {
            await view.OnBeginGameAsync(this, scope);
            await DeckShuffleProcessAsync(scope);
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
            await view.OnBeginTurnAsync(this, user, scope);
            var isWin = await DrawProcessAsync(user, scope);
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
            var index = CurrentUserIndex;
            var user = Users[index];
            await view.OnInvokeAbilityAsync(this, user, Define.CardAbility.Retry, scope);
            var isWin = await DrawProcessAsync(user, scope);
            if (isWin)
            {
                stateMachine.Change(StateEndGame);
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
        
        private UniTask StateEndGame(CancellationToken scope)
        {
            endGameCompletionSource.TrySetResult();
            return UniTask.CompletedTask;
        }

        private async UniTask<bool> DrawProcessAsync(User user, CancellationToken scope)
        {
            if (Deck.IsEmpty())
            {
                await DeckFillProcessAsync(scope);
                await DeckShuffleProcessAsync(scope);
            }
            var card = user.Draw(Deck);
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
            await view.OnDiscardAsync(this, user, card, scope);
            DiscardDeck.Push(card);
            return card;
        }
        
        private async UniTask DeckFillProcessAsync(CancellationToken scope)
        {
            Deck.Fill(DiscardDeck);
            await view.OnFilledDeckAsync(this, scope);
        }
        
        private async UniTask DeckShuffleProcessAsync(CancellationToken scope)
        {
            Deck.Shuffle(random);
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
                default:
                    Assert.IsTrue(false, $"Invalid card ability: {card.Ability}");
                    break;
            }
        }
    }
}

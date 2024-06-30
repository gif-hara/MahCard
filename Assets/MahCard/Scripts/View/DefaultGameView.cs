using System;
using Cysharp.Threading.Tasks;
using HK;
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
                cardInstance.Q<Image>("MainImage").color = game.GameDesignData.GetColor(card.Color);
            }
            return UniTask.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "GameRules", menuName = "MahCard/GameRules")]
    public sealed class GameRules : ScriptableObject
    {
        [SerializeField]
        private int handCardCount;
        public int HandCardCount => handCardCount;

        [SerializeField]
        private DeckBlueprint deckBlueprint;
        public DeckBlueprint DeckBlueprint => deckBlueprint;

        [SerializeField]
        private List<ColorData> colorDatabase;

        [SerializeField]
        private Color currentTurnUserBackgroundColor;
        public Color CurrentTurnUserBackgroundColor => currentTurnUserBackgroundColor;

        [SerializeField]
        private Color currentTurnUserNameColor;
        public Color CurrentTurnUserNameColor => currentTurnUserNameColor;

        [SerializeField]
        private Color defaultUserBackgroundColor;
        public Color DefaultUserBackgroundColor => defaultUserBackgroundColor;

        [SerializeField]
        private Color defaultUserNameColor;
        public Color DefaultUserNameColor => defaultUserNameColor;

        [SerializeField]
        private List<SfxData> sfxDatabase;

        public AudioClip GetSfxClip(Define.SfxType type)
        {
            var data = sfxDatabase.Find(s => s.Type == type);
            if (data == null)
            {
                Assert.IsTrue(false, $"Not found sfx data. type: {type}");
            }

            return data.Clip;
        }

        public ColorData GetColorData(Define.CardColor type)
        {
            var data = colorDatabase.Find(c => c.Type == type);
            if (data == null)
            {
                Assert.IsTrue(false, $"Not found color data. type: {type}");
            }

            return data;
        }

        [Serializable]
        public class ColorData
        {
            [SerializeField]
            private Define.CardColor type;
            public Define.CardColor Type => type;

            [SerializeField]
            private Sprite sprite;
            public Sprite Sprite => sprite;

            [SerializeField]
            private Color color;
            public Color Color => color;
        }

        [Serializable]
        public class SfxData
        {
            [SerializeField]
            private Define.SfxType type;
            public Define.SfxType Type => type;

            [SerializeField]
            private AudioClip clip;
            public AudioClip Clip => clip;
        }
    }
}

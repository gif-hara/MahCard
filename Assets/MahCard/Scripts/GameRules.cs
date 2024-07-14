using System;
using System.Collections.Generic;
using HK;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("sfxDatabase")]
        private AudioData.DictionaryList audioDatabase;

        public AudioClip GetAudioClip(string key) => audioDatabase.Get(key).Clip;

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
        public class AudioData
        {
            [SerializeField]
            private AudioClip clip;
            public AudioClip Clip => clip;

            [Serializable]
            public class DictionaryList : DictionaryList<string, AudioData>
            {
                public DictionaryList() : base(s => s.clip.name)
                {
                }
            }
        }
    }
}

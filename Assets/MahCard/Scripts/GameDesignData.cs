using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "GameDesignData", menuName = "MahCard/GameDesignData")]
    public sealed class GameDesignData : ScriptableObject
    {
        [SerializeField]
        private List<ColorData> colorDatabase;

        public Color GetColor(Define.CardColor type)
        {
            var data = colorDatabase.Find(c => c.Type == type);
            if (data == null)
            {
                Assert.IsTrue(false, $"Not found color data. type: {type}");
            }

            return data.Color;
        }

        [Serializable]
        public class ColorData
        {
            [SerializeField]
            private Define.CardColor type;
            public Define.CardColor Type => type;

            [SerializeField]
            private Color color;
            public Color Color => color;
        }
    }
}

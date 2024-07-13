namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public static class Define
    {
        public enum CardColor
        {
            Red,
            Blue,
            Green,
            White,
            Black
        }

        public enum CardAbility
        {
            None,
            Retry,
            Reset,
            Trade,
            Double,
        }

        public enum DeckType
        {
            Deck,
            DiscardDeck,
        }

        public enum SfxType
        {
            DrawCard,
            DiscardCard,
        }
    }
}

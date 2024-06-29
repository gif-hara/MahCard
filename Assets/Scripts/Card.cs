
namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Card
    {
        public Define.CardColor Color { get; }

        public Define.CardAbility Ability { get; }

        public Card(Define.CardColor color, Define.CardAbility ability)
        {
            Color = color;
            Ability = ability;
        }
    }
}

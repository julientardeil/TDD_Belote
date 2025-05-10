namespace Belote.Tests;

public class CardTests
{
    [Theory]
    [InlineData(SuitEnum.Heart, SuitEnum.Diamond, SuitEnum.Diamond, RankEnum.Jack, 20)]
    [InlineData(SuitEnum.Heart, SuitEnum.Diamond, SuitEnum.Heart, RankEnum.Jack, 5)]
    [InlineData(SuitEnum.Heart, SuitEnum.Diamond, SuitEnum.Spade, RankEnum.Jack, 0)]
    public void Card_Should_Have_Strength(SuitEnum askingColor, SuitEnum trumpColor,SuitEnum cardColor, RankEnum cardValue,  int strength )
    {
        var value = new Rank(cardValue);
        var card = new Card(cardColor, value);
       Assert.Equal(strength, card.GetStrength(askingColor, trumpColor));
    }
   
}

public class ValueTests
{
    [Theory]
    [InlineData(RankEnum.Ace, RankEnum.Ace, true)]
    [InlineData(RankEnum.Jack, RankEnum.Ace, false)]
    public void Values_Can_Be_Compared(RankEnum first, RankEnum second, bool exceptedResult)
    {
        
        Assert.Equal(exceptedResult, new Rank(first) == new Rank( second));
    }
}

public class Card(SuitEnum suit, Rank rank){
    public SuitEnum Suit { get; } = suit;
    public Rank Rank { get; } = rank;

    public int GetStrength(SuitEnum askingColor, SuitEnum trumpColor)
    {
        if (trumpColor == Suit)
        {
            return Rank.GetStrenght(true);
        }
        else if (askingColor != Suit)
        {
            return 0;
        }
        else
        {
            return rank.GetStrenght(false);
        }
    }
}

public enum RankEnum
{
    Ace,
    King,
    Queen,
    Jack,
    Ten,
    Nine,
    Eight,
    Seven
}

public class Rank(RankEnum value)
{
    private readonly RankEnum _rank = value;

    public int GetStrenght(bool trump)
    {
        int result = 0 ;
        if (trump)
        {
            switch (_rank)
            {
                case RankEnum.Jack:
                    result= 20;
                    break;
                case RankEnum.Ace:
                    result= 19;
                    break;
                case RankEnum.King:
                    result= 18;  
                    break;
                case RankEnum.Queen:
                    result= 17;
                    break;
                case RankEnum.Ten:
                    result= 16;
                    break;
                case RankEnum.Nine:
                    result= 15;
                    break;
                case RankEnum.Eight:
                    result= 14;
                    break;
                case RankEnum.Seven:
                    result= 13;
                    break;
            }
        }
        else
        {
            switch (_rank)
            {
                
                case RankEnum.Ace:
                    result = 8;
                    break;
                case RankEnum.King:
                    result = 7;
                    break;
                case RankEnum.Queen:
                    result = 6;
                    break;
                case RankEnum.Jack:
                    result = 5;
                    break;
                case RankEnum.Ten:
                    result = 4;
                    break;
                case RankEnum.Nine:
                    result = 3;
                    break;
                case RankEnum.Eight:
                    result = 2;
                    break;
                case RankEnum.Seven:
                    result = 1;
                    break;
            }
        }
        return result;
    }

    public override bool Equals(object? obj)
    {
        return obj is Rank other && _rank == other._rank;
    }

    public override int GetHashCode()
    {
        return _rank.GetHashCode();
    }

    public static bool operator ==(Rank? left, Rank? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left._rank == right._rank;
    }

    public static bool operator !=(Rank? left, Rank? right)
    {
        return !(left == right);
    }

    public static implicit operator RankEnum(Rank rank) => rank._rank;
    public static implicit operator Rank(RankEnum value) => new Rank(value);
    
}

public enum SuitEnum
{
    Club, 
    Diamond,
    Heart,
    Spade,
}
using Moq;

namespace Belote.Tests;

public class TrickTests
{
    private Mock<ITrick> trickMock = new();
    [Fact]
    public void PlayCard_Add_To_Trick()
    {
        var card = new Card(SuitEnum.Heart, new Rank(RankEnum.Jack));
        var trick = new Trick(SuitEnum.Heart);
        trick.Play(new Player(trickMock.Object), card);
        Assert.Single(trick.Cards);
    }

    [Fact]
    public void PlayCard_First_Card_Define_Color()
    {
        var card = new Card(SuitEnum.Heart, new Rank(RankEnum.Jack));
        var trick = new Trick(SuitEnum.Heart);
        trick.Play(new Player(trickMock.Object), card);
        Assert.Equal(trick.AskedSuit, card.Suit);
    }

    [Theory]
    [InlineData(SuitEnum.Heart, RankEnum.Jack)]
    [InlineData(SuitEnum.Club, RankEnum.Ace)]
    [InlineData(SuitEnum.Diamond, RankEnum.King)]
    [InlineData(SuitEnum.Spade, RankEnum.Eight)]
    public void Should_Select_Winning_Card(SuitEnum trumpColor, RankEnum winningValue)
    {
        var heartJack = new Card(SuitEnum.Heart, new Rank(RankEnum.Jack));
        var heartAce = new Card(SuitEnum.Heart, new Rank(RankEnum.Ace));
        var diamondKing = new Card(SuitEnum.Diamond, new Rank(RankEnum.King));
        var spadeEight = new Card(SuitEnum.Spade, new Rank(RankEnum.Eight));
        var cards = new List<Card>() { heartJack, heartAce, diamondKing, spadeEight };

        var trick = new Trick(trumpColor);
        foreach (var card in cards)
        {
            trick.Play(new Player(trickMock.Object), card);
        }

        Assert.Equal(winningValue, (RankEnum)trick.Winner!.Value.card.Rank);
    }

    [Fact]
    public void ResetTrick_NoCard_NoColor()
    {
        var trumpColor = SuitEnum.Heart;
        var heartJack = new Card(SuitEnum.Heart, new Rank(RankEnum.Jack));
        var heartAce = new Card(SuitEnum.Heart, new Rank(RankEnum.Ace));
        var diamondKing = new Card(SuitEnum.Diamond, new Rank(RankEnum.King));
        var spadeEight = new Card(SuitEnum.Spade, new Rank(RankEnum.Eight));
        var cards = new List<Card>() { heartJack, heartAce, diamondKing, spadeEight };

        var trick = new Trick(trumpColor);
        foreach (var card in cards)
        {
            trick.Play(new Player(trickMock.Object), card);
        }

        trick.Reset();
        Assert.Empty(trick.Cards);
        Assert.Null(trick.AskedSuit);
        Assert.Equal(trumpColor, trick.TrumpSuit);
    }

    [Fact]
    public void Trick_Winner_Player()
    {
        var player1 = new Player(trickMock.Object);
        var player2 = new Player(trickMock.Object);
        var player3 = new Player(trickMock.Object);
        var player4 = new Player(trickMock.Object);
        var heartJack = new Card(SuitEnum.Heart, new Rank(RankEnum.Jack));
        var heartAce = new Card(SuitEnum.Heart, new Rank(RankEnum.Ace));
        var diamondKing = new Card(SuitEnum.Diamond, new Rank(RankEnum.King));
        var spadeEight = new Card(SuitEnum.Spade, new Rank(RankEnum.Eight));
        var playedCards = new List<(Player player, Card card)>()
        {
            (player1, heartJack),
            (player2, heartAce),
            (player3, diamondKing),
            (player4, spadeEight)
        };

        var trick = new Trick(SuitEnum.Heart);
        foreach (var playedCard in playedCards)
        {
            trick.Play(playedCard.player, playedCard.card);
        }

        Assert.Equal(player1, trick.Winner!.Value.player);
    }
}

public interface ITrick
{
    SuitEnum? AskedSuit { get; }
    (Player player, Card card)? Winner { get; }
    void Play(Player player, Card card);
    void Reset();
}

public class Trick(SuitEnum trumpSuit) : ITrick
{
    public readonly List<( Player player, Card card)> Cards = [];
    public readonly SuitEnum TrumpSuit = trumpSuit;
    public SuitEnum? AskedSuit { get; private set; }

    public (Player player, Card card)? Winner
    {
        get
        {
            return AskedSuit is null
                ? null
                : Cards.OrderByDescending(i => i.card.GetStrength(AskedSuit.Value, TrumpSuit)).First();
        }
    }

    public void Play(Player player, Card card)
    {
        AskedSuit ??= card.Suit;
        Cards.Add((player,card));
    }

    public void Reset()
    {
        AskedSuit = null;
        Cards.Clear();
    }
}
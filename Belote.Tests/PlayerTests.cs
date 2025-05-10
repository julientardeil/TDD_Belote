using Moq;

namespace Belote.Tests;

public class PlayerTests
{
    private readonly Mock<ITrick> trickMock = new();

    [Fact]
    public void PlayStartWithEmptyHand()
    {
        var player = new Player(trickMock.Object);
        Assert.Empty(player.Hand);
    }

    [Fact]
    public void Player_ReceiveCard_AddToHand()
    {
        var player = new Player(trickMock.Object);
        var cards = new List<Card>() {new Card(SuitEnum.Club, RankEnum.Ace),  new Card(SuitEnum.Club, RankEnum.Ace), new Card(SuitEnum.Club, RankEnum.Ace)};
        player.Receive(cards);
        Assert.Equal(3, player.Hand.Count);
    }

    [Fact]
    public void Player_Cannot_Received_More_Than_8_Cards()
    {
        var player = new Player(trickMock.Object);
        var cards = new List<Card>() {new Card(SuitEnum.Club, RankEnum.Ace),  new Card(SuitEnum.Club, RankEnum.Ace), new Card(SuitEnum.Club, RankEnum.Ace)};
        player.Receive(cards);
        player.Receive(cards);
        Assert.Throws<InvalidOperationException>(() => player.Receive(cards));
    }

    [Fact]
    public void Player_Can_Play_Card()
    {
        var playedCards = new List<(Player player, Card card)>();
        trickMock.Setup(i => i.Play(It.IsAny<Player>(), It.IsAny<Card>())).Callback<Player, Card>((player, card) =>
        {
            playedCards.Add((player, card));
        });
        var player = new Player(trickMock.Object);
        var card = new Card(SuitEnum.Club, RankEnum.Ace);
        var cards = new  List<Card>() {card};
        player.Receive(cards);
        player.Play(0);
        Assert.Single(playedCards); 
        Assert.Equal(player, playedCards[0].player);
        Assert.Equal(card.Suit, playedCards[0].card.Suit);
        Assert.Equal(card.Rank, playedCards[0].card.Rank);
    }

    [Fact]
    public void PlayerPlayLastCard_NoCardLeft()
    {
        var player = new Player(trickMock.Object);
        var card = new Card(SuitEnum.Club, RankEnum.Ace);
        var cards = new List<Card>() {card};
        player.Receive(cards);
        player.Play(0);
        Assert.Empty(player.Hand);
    }

    [Fact]
    public void PlayerPlayNotExistingCard_Throw_InvalidOperationException()
    {
        var player = new Player(trickMock.Object);
        var card = new Card(SuitEnum.Club, RankEnum.Ace);
        var cards = new List<Card>() {card};
        player.Receive(cards);
        Assert.Throws<InvalidOperationException>(() => player.Play(1));
    }

    [Fact]
    public void PlayerPlayInvalidAskedColor_Throw_InvalidOperationException()
    {
        trickMock.Setup(i => i.AskedSuit).Returns(() => SuitEnum.Club);
        var player = new Player(trickMock.Object);
        var validCard = new Card(SuitEnum.Club, RankEnum.Ace);
        var invalidCard = new Card(SuitEnum.Heart, RankEnum.Ace);
        var cards = new List<Card>() {validCard, invalidCard};
        player.Receive(cards);
        Assert.Throws<InvalidOperationException>(() => player.Play(1));
    }
}

public sealed class Player(ITrick trick)
{
    public List<Card> Hand { get; set; } = [];

    public void Receive(List<Card>cards)
    {
        if (Hand.Count + cards.Count > 8)
        {
            throw new InvalidOperationException();
        }
        Hand.AddRange(cards);
    }

    public void Play(int cardIndex)
    {
        if (cardIndex > Hand.Count - 1)
        {
            throw new InvalidOperationException($"No card at index {cardIndex}");
        }
        var selectedCard = Hand[cardIndex];
        if (!IsValidCard(selectedCard))
        {
            throw new InvalidOperationException($"Invalid played card at index {cardIndex}");
        }
        trick.Play(this, selectedCard);
        Hand.Remove(selectedCard);
    }

    private bool IsValidCard(Card card)
    {
        if(trick.AskedSuit != card.Suit && Hand.Any(i=> i.Suit == trick.AskedSuit))
        {
            return false;
        }
        return true;
    }
}
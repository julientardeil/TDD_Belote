using Moq;

namespace Belote.Tests;

public class DealerTests
{
    private Mock<ITrick> trick = new();
    
    [Fact]
    public void Dealer_Should_Distribute_Card_To_All_Players()
    {
        var player1 = new Player(trick.Object);
        var player2 = new Player(trick.Object);
        var player3 = new Player(trick.Object);
        var player4 = new Player(trick.Object);
        var players = new List<Player> { player1, player2, player3, player4 };
        var deck = new Deck();
        var dealer = new Dealer(deck, players);
        dealer.Deal();

        foreach (var player in players)
        {
            Assert.Equal(8, player.Hand.Count);
        }
    }
}

public class Dealer(Deck deck, List<Player> players)
{
    private static readonly int [] CardCountOrder = [3, 2, 3];

    public void Deal()
    {
        foreach (var cardCount in CardCountOrder)
        {
            foreach (var player in players)
            {
                var draw = deck.Draw(cardCount);
                player.Receive(draw);
            }
        }
    }
}
using static System.Enum;

namespace Belote.Tests;

public class DeckTests
{
    [Fact]
    public void Deck_Be_Drawn()
    {
        var deck = new Deck();
        var cards = deck.Draw(2);
        Assert.Equal(2, cards.Count);
        
    }
[Fact]
    public void Deck_Contains_All_Cards()
    {
        var deck = new Deck();
        var cards = deck.Draw(32);
        Assert.Equal(32, cards.Count);
        var distinctCards = cards.GroupBy(i => new { i.Rank, Color = i.Suit});
        Assert.Equal(32, distinctCards.Count());
    }
[Fact]
    public void Draw_Never_Give_Same_Card()
    {
        var deck = new Deck();
        var cards =new List<Card>();
        for (int i = 0; i < 32; i++)
        {
            cards.AddRange(deck.Draw(1));
        }
        var distinctCards = cards.GroupBy(i => new { i.Rank, Color = i.Suit});
        Assert.Equal(32, distinctCards.Count());
    }

    [Fact]
    public void Draw_Throw_Exception_If_Not_Enough_Card()
    {
        var deck = new Deck();
        deck.Draw(32);
        Assert.Throws<InvalidOperationException>(() => deck.Draw(2));
    }
}

public class Deck
{
    private readonly List<Card> _cards = [];
    public Deck()
    {
        foreach (var color in GetValues<SuitEnum>())
        {
            foreach (var rank in GetValues<RankEnum>())
            {
                var card = new Card(color, rank);
                _cards.Add(card);
            }
        }
    }
    public List<Card> Draw(int count)
    {
        if (_cards.Count < count)
        {
            throw new InvalidOperationException("Not enough cards");
        }
        var draw = _cards.GetRange(0, count);   
        _cards.RemoveRange(0,count);
        return draw;
    }
}
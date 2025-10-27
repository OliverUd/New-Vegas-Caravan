namespace CaravanGame;

public class Player
{
    public readonly PlayerPosition Position;
    public readonly PlayerType Type;
    public readonly Queue<Card> Deck;
    public readonly List<Card> Hand;
    public readonly Caravan[] Caravans;

    public Player(PlayerPosition position, PlayerType type)
    {
        Position = position;
        Type = type;
        Caravans = [new() { Owner = this }, new() { Owner = this }, new() { Owner = this }];
        Deck = new(Card.GenerateDeck(shuffle: true));
        Hand = InitialiseHand();
    }

    public List<Card> InitialiseHand()
    {
        List<Card> hand = new(8);
        int numCards = 0;
        for (int card = 0; card <= 8; card++)
        {
            hand.Add(Deck.Dequeue());
            if (hand[card].Value <= CardValue.Ten)
            {
                (hand[card], hand[numCards]) = (hand[numCards], hand[card]);
                numCards++;
            }
        }
        while (numCards < 3)
        {
            Card card = Deck.Peek();
            if (card.Value <= CardValue.Ten)
            {
                Deck.Enqueue(hand[7]);
                hand.RemoveAt(7);
                hand.Insert(numCards, card);
                numCards++;
            }
        }
        return hand;
    }
}

public enum PlayerType
{
    Human,
    AI
}

public enum PlayerPosition
{
    Top,
    Bottom
}
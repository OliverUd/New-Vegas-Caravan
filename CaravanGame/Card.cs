namespace CaravanGame
{
    public class Card
    {
        public Suit Suit { get; }
        public CardValue Value { get; }
        public string Image { get; }

        public static Dictionary<CardValue, Dictionary<Suit, Card>> FullDeck { get; } = [];

        public Card(Suit suit, CardValue value)
        {
            Suit = suit;
            Value = value;
            Image = "CardImages/" + Enum.GetName(typeof(CardValue), (int) value) + Enum.GetName(typeof(Suit), (int) suit) + ".svg";
        }

        public Card()
        {
            Image = "CardImages/Back.svg";
        }

        static Card()
        {
            foreach (Card card in GenerateDeck())
            {
                if (!FullDeck.ContainsKey(card.Value)) FullDeck[card.Value] = [];
                FullDeck[card.Value][card.Suit] = card;
            }
        }

        public static Card[] GenerateDeck()
        {
            Card[] cards = new Card[54];
            foreach (int card in Enumerable.Range(0, 54)) { cards[card] = new((Suit)(card % 4), (CardValue)(card / 4 + 1)); }
            return cards;
        }
    }
}
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

        public static Card[] GenerateDeck(bool shuffle = false)
        {
            Card[] cards = new Card[54];
            foreach (int card in Enumerable.Range(0, 54)) { cards[card] = new((Suit)(card % 4), (CardValue)(card / 4 + 1)); }
            if (shuffle) cards = (Card[])Shuffle(cards);
            return cards;
        }

        public static IList<T> Shuffle<T>(IList<T> list)
        {
            Random rand = new();
            foreach (int item in Enumerable.Range(0, list.Count))
            {
                int swap = rand.Next(54);
                (list[item], list[swap]) = (list[swap], list[item]);
            }
            return list;
        }
    }
}
namespace CaravanGame
{
    public static class Game
    {
        public static Card? SelectedCard { get; set; }

        public static readonly string[] Players = ["Player", "AI"];

        public static Dictionary<string, Queue<Card>> Decks { get; } = new(2);
        public static Dictionary<string, List<Card>> Hands { get; } = new(2);
        public static Caravan[] AllCaravans { get; } = [
            new Caravan { Owner = Players[0] },
            new Caravan { Owner = Players[0] },
            new Caravan { Owner = Players[0] },
            new Caravan { Owner = Players[1] },
            new Caravan { Owner = Players[1] },
            new Caravan { Owner = Players[1] }
        ];
        public static Dictionary<string, Caravan[]> Caravans { get; } = new()
        {
            { Players[0], AllCaravans.Take(3).ToArray() },
            { Players[1], AllCaravans.Skip(3).ToArray() }
        };

        public static char Winner { get; set; } = 'N';

        public static bool TryPlayCard(string currentPlayer, Caravan selectedCaravan, int position)
        {
            if (!selectedCaravan.ValidMove(currentPlayer, SelectedCard, position)) return false;
            Card? joker = selectedCaravan.AddCard(SelectedCard!, position);

            Hands[currentPlayer].Remove(SelectedCard!);
            SelectedCard = null;
            if (Hands[currentPlayer].Count < 5 && Decks[currentPlayer].Count != 0) Hands[currentPlayer].Add(Decks[currentPlayer].Dequeue());

            if (joker is not null) foreach (Caravan caravan in Caravans["Player"].Concat(Caravans["AI"])) caravan.JokerRemove(joker);

            Winner = CheckWin();
            return true;
        }

        public static void DiscardCaravan(Caravan caravan) => caravan.Reset();

        public static char CheckWin()
        {
            short winner = 0;
            Caravan AICaravan, PlayerCaravan;
            for (int caravan = 0; caravan < 3; caravan++)
            {
                PlayerCaravan = Caravans["Player"][caravan];
                AICaravan = Caravans["AI"][caravan];
                if (!(AICaravan.Value == PlayerCaravan.Value) && (AICaravan.Sold || PlayerCaravan.Sold))
                {
                    winner += Convert.ToInt16(AICaravan.Value < PlayerCaravan.Value);
                }
                else return 'N';
            }
            return winner >= 2 ? 'P' : 'A';
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

        public static void Initialise()
        {
            InitialiseDecks();
            InitialiseHands();
        }

        public static void InitialiseDecks(bool shuffle = true)
        {
            Card[] cards = Card.GenerateDeck();
            foreach (string player in Players)
            {
                if (shuffle) cards = (Card[]) Shuffle(cards);
                Decks[player] = new(cards);
            }
        }

        public static void InitialiseHands()
        {
            foreach (string player in Players)
            {
                Hands[player] = new(8);
                int numCards = 0;
                foreach (int card in Enumerable.Range(0, 8))
                {
                    Hands[player].Add(Decks[player].Dequeue());
                    if (Hands[player][card].Value <= CardValue.Ten)
                    {
                        (Hands[player][card], Hands[player][numCards]) = (Hands[player][numCards], Hands[player][card]);
                        numCards++;
                    }
                }
                while (numCards < 3)
                {
                    Card card = Decks[player].Peek();
                    if (card.Value <= CardValue.Ten)
                    {
                        Decks[player].Enqueue(Hands[player][7]);
                        Hands[player].RemoveAt(7);
                        Hands[player].Insert(numCards, card);
                        numCards++;
                    }
                }
            }
        }
    }
}
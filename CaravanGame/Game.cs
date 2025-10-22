using CaravanGame.Bots;

namespace CaravanGame;

public static class Game
{
    public static Card? SelectedCard { get; set; }

    public static readonly string[] Players = ["Player", "AI"];

    public static string CurrentPlayer { get; private set; } = Players[0];

    public static Bot Bot { get; private set; }

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

    public static bool TryPlayMove(Move move)
    {
        if (move.Player != CurrentPlayer || !move.Caravan!.ValidMove(move)) return false;
        Card? joker = move.Caravan.AddCard(SelectedCard!, move.Position);

        Hands[CurrentPlayer].Remove(SelectedCard!);
        SelectedCard = null;
        if (Hands[CurrentPlayer].Count < 5 && Decks[CurrentPlayer].Count != 0) Hands[CurrentPlayer].Add(Decks[CurrentPlayer].Dequeue());

        if (joker is not null) foreach (Caravan caravan in AllCaravans) caravan.JokerRemove(joker);

        NextTurn();
        return true;
    }

    public static void DiscardCaravan(Caravan caravan)
    {
        caravan.Reset();
        NextTurn();
    }

    public static void DiscardCard()
    {
        Hands[CurrentPlayer].Remove(SelectedCard!);
        SelectedCard = null;
        NextTurn();
    }

    public static void NextTurn()
    {
        Winner = CheckWin();
        if (CurrentPlayer == Players[0])
        {
            CurrentPlayer = Players[1];


        }
    }

    public static char CheckWin()
    {
        short winner = 0;
        Caravan aiCaravan, playerCaravan;
        for (int caravan = 0; caravan < 3; caravan++)
        {
            playerCaravan = Caravans["Player"][caravan];
            aiCaravan = Caravans["AI"][caravan];
            if (!(aiCaravan.Value == playerCaravan.Value) && (aiCaravan.Sold || playerCaravan.Sold))
            {
                winner += Convert.ToInt16(aiCaravan.Value < playerCaravan.Value);
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

public enum MoveType
{
    PlayCard,
    DiscardCard,
    DiscardCaravan
}

public class Move
{
    public readonly string Player;
    public readonly MoveType Type;
    public readonly Caravan? Caravan;
    public readonly Card? Card;
    public readonly int? Position;

    public Move(string player, MoveType type, Caravan? caravan = null, Card? card = null, int? position = null)
    {
        Player = player;
        Type = type;

        switch (type)
        {
            case MoveType.DiscardCard:
                if (card is null) throw new ArgumentException($"{nameof(card)} must not be null if {nameof(type)} is {nameof(MoveType.DiscardCard)}");
                Card = card;
                return;
            case MoveType.DiscardCaravan:
                if (caravan is null) throw new ArgumentException($"{nameof(caravan)} must not be null if {nameof(type)} is {nameof(MoveType.DiscardCaravan)}");
                Caravan = caravan;
                return;
            case MoveType.PlayCard:
                if (caravan is null || card is null) throw new ArgumentException($"{nameof(caravan)} and {nameof(card)} must not be null if {nameof(type)} is {nameof(MoveType.PlayCard)}");
                if (position is null && card.Value > CardValue.Ten) throw new ArgumentException($"{nameof(position)} must have a value for face cards");
                Caravan = caravan;
                Card = card;
                Position = position;
                return;
        }
    }
}
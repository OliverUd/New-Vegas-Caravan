namespace CaravanGame;

public static class Game
{
    public static Card? SelectedCard { get; set; }

    public static readonly Dictionary<PlayerPosition, Player> Players = new() { { PlayerPosition.Top, new(PlayerPosition.Top, PlayerType.AI) }, { PlayerPosition.Bottom, new(PlayerPosition.Bottom, PlayerType.Human) } };

    public static Player CurrentPlayer { get; private set; } = Players[PlayerPosition.Bottom];

    public static char Winner { get; set; } = 'N';

    public static bool TryPlayMove(Move move)
    {
        if (move.Player != CurrentPlayer) return false;

        if (move.Type == MoveType.DiscardCard)
        {
            move.Player.Hand.Remove(move.Card!);
            if (move.Card == SelectedCard) SelectedCard = null;
        }

        else if (!move.Caravan!.ValidMove(move)) return false;

        else if (move.Type == MoveType.DiscardCaravan)
        {
            move.Caravan.Reset();
        }

        else
        {
            Card? joker = move.Caravan.AddCard(move.Card!, move.Position);

            move.Player.Hand.Remove(move.Card!);
            if (move.Card == SelectedCard) SelectedCard = null;
            if (move.Player.Hand.Count < 5 && move.Player.Deck.Count != 0) move.Player.Hand.Add(move.Player.Deck.Dequeue());

            if (joker is not null)
            {
                foreach (Caravan caravan in Players[PlayerPosition.Top].Caravans) caravan.JokerRemove(joker);
                foreach (Caravan caravan in Players[PlayerPosition.Bottom].Caravans) caravan.JokerRemove(joker);
            }
        }

        NextTurn();
        return true;
    }

    public static void NextTurn()
    {
        Winner = CheckWin();
        if (CurrentPlayer == Players[PlayerPosition.Top])
        {
            CurrentPlayer = Players[PlayerPosition.Bottom];
            if (CurrentPlayer.Type == PlayerType.Human) return;
        } else CurrentPlayer = Players[PlayerPosition.Top];
    }

    public static char CheckWin()
    {
        short winner = 0;
        Caravan topCaravan, bottomCaravan;
        for (int caravan = 0; caravan < 3; caravan++)
        {
            topCaravan = Players[PlayerPosition.Top].Caravans[caravan];
            bottomCaravan = Players[PlayerPosition.Bottom].Caravans[caravan];
            if (!(topCaravan.Value == bottomCaravan.Value) && (topCaravan.Sold || bottomCaravan.Sold))
            {
                winner += Convert.ToInt16(topCaravan.Value < bottomCaravan.Value);
            }
            else return 'N';
        }
        return winner >= 2 ? 'P' : 'A';
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
    public readonly Player Player;
    public readonly MoveType Type;
    public readonly Caravan? Caravan;
    public readonly Card? Card;
    public readonly int? Position;

    public Move(Player player, MoveType type, Caravan? caravan = null, Card? card = null, int? position = null)
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
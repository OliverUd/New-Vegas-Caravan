 namespace CaravanGame
{
    public class Caravan
    {
        public List<Stack> Cards { get; } = [];
        public int Value { get; private set; } = 0;
        public Direction Direction { get; private set; } = Direction.None;
        public bool Sold { get => Value >= 21 && Value <= 26; }
        public required string Owner { get; set; }

        public List<int> ValidMoves(string player, Card card)
        {
            List<int> validMoves = [];
            if (card.Value > CardValue.Ten)
            {
                for (int stack = 0; stack < Cards.Count; stack++)
                {
                    if (ValidMove(new(player, this, card, stack))) validMoves.Add(stack);
                }
            }
            else if (ValidMove(new(player, this, card))) validMoves.Add(Cards.Count);
            return validMoves;
        }

        public bool ValidMove(Move move)
        {
            if (move.Card.Value > CardValue.Ten)
            {
                if (move.Position is null) throw new ArgumentException("Face cards must have a position specified");
                if (Cards.Count == 0) return false;
                return true;
            }

            if (Owner == move.Player
            && (Direction == Direction.None
            || (Direction == Direction.Up && move.Card.Value > Cards.Last().BaseCard.Value)
            || (Direction == Direction.Down && move.Card.Value < Cards.Last().BaseCard.Value)
            || move.Card.Suit == Cards.Last().BaseCard.Suit)) return true;
            return false;
        }

        public Card? AddCard(Card card, int? position = null)
        {
            if (card.Value <= CardValue.Ten)
            {
                if (Cards.Count > 0) Direction = card.Value > Cards.Last().BaseCard.Value ? Direction.Up : Direction.Down;
                Cards.Add(new Stack(card));
                Value += (int)card.Value;
            }
            else if (position is null) throw new ArgumentException("Face cards must have a position specified");
            else switch (card.Value)
                {
                    case CardValue.Jack:
                        Value -= Cards[(int)position].Value;
                        Cards.RemoveAt((int)position);
                        break;
                    case CardValue.Queen:
                        Cards[(int)position].Modifiers.Add(card);
                        Direction = Direction == Direction.None ? Direction.None : Direction == Direction.Up ? Direction.Down : Direction.Up;
                        break;
                    case CardValue.King:
                        Cards[(int)position].Modifiers.Add(card);
                        Value += Cards[(int)position].Value;
                        break;
                    case CardValue.Joker:
                        Cards[(int)position].Modifiers.Add(card);
                        Cards[(int)position].NewJoker = true;
                        return Cards[(int)position].BaseCard;
                }
            return null;
        }

        public void JokerRemove(Card removeCard)
        {
            dynamic removeVal = removeCard.Value == CardValue.Ace ? removeCard.Suit : removeCard.Value;
            Func<Card, dynamic> cardVal = removeCard.Value == CardValue.Ace ? (card => card.Suit) : (card => card.Value);
            Cards.RemoveAll(delegate (Stack stack)
            {
                bool val = removeVal.Equals(cardVal(stack.BaseCard)) && !stack.NewJoker;
                if (val) Value -= stack.Value;
                stack.NewJoker = false;
                return val;
            });
        }

        public void Reset()
        {
            Cards.Clear();
            Value = 0;
            Direction = Direction.None;
        }

        public List<Card> CardsToSell()
        {
            List<Card> cards = [];
            if (Value < 16 || Sold) return [];
            int maxAddition = 26 - Value;
            int minAddition = 21 - Value;

            if (maxAddition > 0)
            {
                Suit currentSuit = Cards.Last().BaseCard.Suit;
                for (int cardValue = minAddition; cardValue <= maxAddition; cardValue++)
                {
                    cards.Add(Card.FullDeck[(CardValue)cardValue][currentSuit]);
                    if (Direction == Direction.Up && cardValue > (int)Cards.Last().BaseCard.Value)
                    {
                        foreach (Suit suit in Enum.GetValues<Suit>().Where((suitValue) => suitValue != currentSuit))
                        {
                            cards.Add(Card.FullDeck[(CardValue)cardValue][suit]);
                        }
                    }
                }
            }

            else if (Cards.Any((stack) => stack.Value >= 0 - maxAddition))
            {
                foreach (Card card in Card.FullDeck[CardValue.Jack].Values) cards.Add(card);
            }

            return cards;
        }
    }

    public class Stack(Card card)
    {
        public int Value 
        {
            get
            {
                int kingCount = 0;
                foreach (var card in Modifiers) { if (card.Value == CardValue.King) kingCount++; }
                return kingCount == 0 ? (int) BaseCard.Value : (int) Math.Pow((double) BaseCard.Value, kingCount);
            }
        }
        public Card BaseCard { set; get; } = card;
        public List<Card> Modifiers { set; get; } = [];
        public bool NewJoker { get; set; } = false;
    }

    public enum Direction { None, Up, Down }
}
namespace CaravanGame.Bots
{
    public class Bot
    {
        public int Turns { get; set; } = 0;
        public List<Card> Hand { get => Game.Hands[Game.Players[1]]; }

        public List<int>[,] FindValidMoves()
        {
            List<int>[,] validMoves = new List<int>[Hand.Count, 6];
            for (int card = 0; card < Hand.Count; card++)
            {
                int caravanCount = Hand[card].Value > CardValue.Ten ? 6 : 3;
                for (int car = 0; car < caravanCount; car++)
                {
                    validMoves[card, car].AddRange(Game.AllCaravans[car].ValidMoves(Game.Players[1], Hand[card]));
                }
            }
            return validMoves;
        }

        public float CaravanSellChance(Caravan caravan)
        {
            throw new NotImplementedException();
        }

        public void PlayFirstMoves()
        {

        }

        public void PlayMove()
        {

        }
    }
}
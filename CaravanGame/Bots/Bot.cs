namespace CaravanGame.Bots
{
    public abstract class Bot(string name, List<Card> hand, List<Caravan> caravans)
    {
        private string name = name;
        private List<Card> hand = hand;
        private List<Caravan> caravans = caravans;

        private List<int>[,] GetValidMoves()
        {
            List<int>[,] validMoves = new List<int>[hand.Count, 6];
            for (int card = 0; card < hand.Count; card++)
            {
                int caravanCount = hand[card].Value > CardValue.Ten ? 6 : 3;
                for (int caravan = 0; caravan < caravanCount; caravan++)
                {
                    validMoves[card, caravan].AddRange(caravans[caravan].ValidMoves(name, hand[card]));
                }
            }
            return validMoves;
        }

        public void PlayFirstMoves()
        {

        }

        public abstract Move ChooseMove();
    }
}
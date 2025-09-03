namespace CaravanGame.Bots
{
    public class RandomBot(string name, List<Card> hand, List<Caravan> caravans) : Bot(name, hand, caravans)
    {
        public override Move ChooseMove()
        {
            throw new NotImplementedException();
        }
    }
}
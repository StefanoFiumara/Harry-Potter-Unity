namespace HarryPotterUnity.Cards.Charms.Locations
{
    public class Gringotts : BaseLocation
    {
        public override void OnInPlayBeforeTurnAction()
        {
            Player.AddActions(1);
        }

        public override void OnEnterInPlayAction()
        {
            Player.AddActions(1);
        }

        public override void OnInPlayAfterTurnAction()
        {
            Player.OppositePlayer.AddActions(1);
        }
    }
}
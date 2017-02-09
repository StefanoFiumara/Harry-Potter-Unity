namespace HarryPotterUnity.Cards.Charms.Locations
{
    public class Gringotts : BaseLocation
    {
        public override void OnInPlayBeforeTurnAction()
        {
            this.Player.AddActions(1);
        }

        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();
            this.Player.AddActions(1);
        }

        public override void OnInPlayAfterTurnAction()
        {
            base.OnEnterInPlayAction();
            this.Player.OppositePlayer.AddActions(1);
        }
    }
}
namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class Unicorn : BaseCreature {

        public override void OnInPlayBeforeTurnAction()
        {
            this.Player.AddActions(1);
        }

        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();
            this.Player.AddActions(1);
        }
    }
}

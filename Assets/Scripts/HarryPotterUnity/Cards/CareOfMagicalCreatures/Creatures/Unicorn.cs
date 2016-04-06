namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class Unicorn : BaseCreature {

        public override void OnInPlayBeforeTurnAction()
        {
            Player.AddActions(1);
        }

        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();
            Player.AddActions(1);
        }
    }
}

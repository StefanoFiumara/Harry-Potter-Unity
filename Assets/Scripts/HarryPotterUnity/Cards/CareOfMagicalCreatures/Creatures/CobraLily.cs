namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class CobraLily : BaseCreature
    {
        //TODO: Test this
        public override void OnInPlayAfterTurnAction()
        {
            Heal(MaxHealth);
        }
    }
}
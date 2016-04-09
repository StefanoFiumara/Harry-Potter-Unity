namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class CobraLily : BaseCreature
    {
        public override void OnInPlayAfterTurnAction()
        {
            Heal(MaxHealth);
        }
    }
}
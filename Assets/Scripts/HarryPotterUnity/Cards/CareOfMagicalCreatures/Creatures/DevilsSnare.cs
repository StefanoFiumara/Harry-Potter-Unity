namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class DevilsSnare : BaseCreature
    {
        public override void OnInPlayAfterTurnAction()
        {
            this.Heal(this.MaxHealth);
        }
    }
}
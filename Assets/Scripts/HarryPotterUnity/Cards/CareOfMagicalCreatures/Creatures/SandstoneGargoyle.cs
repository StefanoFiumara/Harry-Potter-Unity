namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class SandstoneGargoyle : BaseCreature
    {
        private bool _hasAddedDamage;

        public override void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.Creatures.Count != 0) return;

            _damagePerTurn += 2;
            _attackLabel.text = (_damagePerTurn).ToString();
            _hasAddedDamage = true;
        }

        public override void OnInPlayAfterTurnAction()
        {
            if (!_hasAddedDamage) return;

            _damagePerTurn -= 2;
            _attackLabel.text = (_damagePerTurn).ToString();
            _hasAddedDamage = false;
        }
    }
}

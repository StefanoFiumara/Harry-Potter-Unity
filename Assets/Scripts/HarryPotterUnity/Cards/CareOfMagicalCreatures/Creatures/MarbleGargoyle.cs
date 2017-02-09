namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class MarbleGargoyle : BaseCreature 
    {
        private bool _hasAddedDamage;

        public override void OnInPlayBeforeTurnAction()
        {
            if (this.Player.OppositePlayer.InPlay.Creatures.Count != 0) return;

            this._damagePerTurn += 3;
            this._attackLabel.text = this._damagePerTurn.ToString();
            this._hasAddedDamage = true;
        }

        public override void OnInPlayAfterTurnAction()
        {
            if (!this._hasAddedDamage) return;

            this._damagePerTurn -= 3;
            this._attackLabel.text = this._damagePerTurn.ToString();
            this._hasAddedDamage = false;
        }
    }
}

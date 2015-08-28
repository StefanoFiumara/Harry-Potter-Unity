using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    [UsedImplicitly]
    public class SandstoneGargoyle : GenericCreature, IPersistentCard
    {
        private bool _hasAddedDamage;

        public new void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count != 0) return;

            Player.DamagePerTurn += 2;
            _attackLabel.text = (_damagePerTurn + 2).ToString();
            _hasAddedDamage = true;
        }

        public new void OnInPlayAfterTurnAction()
        {
            if (!_hasAddedDamage) return;

            Player.DamagePerTurn -= 2;
            _attackLabel.text = (_damagePerTurn - 2).ToString();
            _hasAddedDamage = false;
        }

        public new void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            if (!_hasAddedDamage) return;

            Player.DamagePerTurn -= 2;
            _hasAddedDamage = false;
        }
    }
}

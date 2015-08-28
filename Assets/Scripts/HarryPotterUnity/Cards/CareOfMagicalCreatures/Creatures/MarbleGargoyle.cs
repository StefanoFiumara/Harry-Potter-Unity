using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    [UsedImplicitly]
    public class MarbleGargoyle : GenericCreature, IPersistentCard 
    {
        private bool _hasAddedDamage;

        public new void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count != 0) return;

            Player.DamagePerTurn += 3;
            _attackLabel.text = (_damagePerTurn + 3).ToString();
            _hasAddedDamage = true;
        }

        public new void OnInPlayAfterTurnAction()
        {
            if (!_hasAddedDamage) return;

            Player.DamagePerTurn -= 3;
            _attackLabel.text = (_damagePerTurn).ToString();
            _hasAddedDamage = false;
        }

        public new void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            if (!_hasAddedDamage) return;

            Player.DamagePerTurn -= 3;
            _hasAddedDamage = false;
        }
    }
}

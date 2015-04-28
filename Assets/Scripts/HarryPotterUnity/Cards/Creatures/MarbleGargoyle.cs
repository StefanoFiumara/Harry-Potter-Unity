using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Creatures
{
    [UsedImplicitly]
    public class MarbleGargoyle : GenericCreature, IPersistentCard 
    {
        private bool _hasAddedDamage;

        public new void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count != 0) return;

            Player.DamagePerTurn += 3;
            _hasAddedDamage = true;
        }

        public new void OnInPlayAfterTurnAction()
        {
            if (!_hasAddedDamage) return;

            Player.DamagePerTurn -= 3;
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

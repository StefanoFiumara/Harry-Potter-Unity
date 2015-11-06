using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    [UsedImplicitly]
    public class SandstoneGargoyle : BaseCreature, IPersistentCard
    {
        private bool _hasAddedDamage;

        public new void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count() != 0) return;

            _damagePerTurn += 2;
            _attackLabel.text = (_damagePerTurn).ToString();
            _hasAddedDamage = true;
        }

        public new void OnInPlayAfterTurnAction()
        {
            if (!_hasAddedDamage) return;

            _damagePerTurn -= 2;
            _attackLabel.text = (_damagePerTurn).ToString();
            _hasAddedDamage = false;
        }
    }
}

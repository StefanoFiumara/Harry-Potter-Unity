using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    [UsedImplicitly]
    public class MarbleGargoyle : BaseCreature, IPersistentCard 
    {
        private bool _hasAddedDamage;

        public new void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count() != 0) return;
            
            _damagePerTurn += 3;
            _attackLabel.text = _damagePerTurn.ToString();
            _hasAddedDamage = true;
        }

        public new void OnInPlayAfterTurnAction()
        {
            if (!_hasAddedDamage) return;

            _damagePerTurn -= 3;
            _attackLabel.text = _damagePerTurn.ToString();
            _hasAddedDamage = false;
        }
    }
}

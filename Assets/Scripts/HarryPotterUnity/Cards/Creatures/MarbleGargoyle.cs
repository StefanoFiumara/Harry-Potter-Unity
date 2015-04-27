using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Creatures
{
    [UsedImplicitly]
    public class MarbleGargoyle : GenericCreature, IPersistentCard {

        public new void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count == 0)
            {
                Player.DamagePerTurn += 3;
            }
        }

        public new void OnInPlayAfterTurnAction()
        {
            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count == 0)
            {
                Player.DamagePerTurn -= 3;
            }
        }

        public new void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            if (Player.OppositePlayer.InPlay.GetCreaturesInPlay().Count == 0)
            {
                Player.DamagePerTurn -= 3;
            }
        }
    }
}

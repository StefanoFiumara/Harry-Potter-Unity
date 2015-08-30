using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class TransfigurationExam : GenericSpell
    {
        protected override void SpellAction(List<GenericCard> targets)
        {
            var playerCreatures = Player.InPlay.GetCreaturesInPlay();
            var enemyCreatures = Player.OppositePlayer.InPlay.GetCreaturesInPlay();

            Player.OppositePlayer.Discard.AddAll(enemyCreatures);
            Player.Discard.AddAll(playerCreatures);
            
            Player.OppositePlayer.InPlay.RemoveAll(enemyCreatures);
            Player.InPlay.RemoveAll(playerCreatures);
        }
    }
}

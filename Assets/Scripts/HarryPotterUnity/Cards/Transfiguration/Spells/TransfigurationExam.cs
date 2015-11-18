using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class TransfigurationExam : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.InPlay.GetCreaturesInPlay()
                                .Concat(Player.OppositePlayer.InPlay.GetCreaturesInPlay())
                                .Any();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var playerCreatures = Player.InPlay.GetCreaturesInPlay();
            var enemyCreatures = Player.OppositePlayer.InPlay.GetCreaturesInPlay();

            Player.OppositePlayer.Discard.AddAll(enemyCreatures);
            Player.Discard.AddAll(playerCreatures);
        }
    }
}

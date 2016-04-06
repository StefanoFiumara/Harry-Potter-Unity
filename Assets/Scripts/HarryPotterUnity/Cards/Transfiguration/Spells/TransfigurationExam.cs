using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class TransfigurationExam : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.InPlay.Creatures
                                .Concat(Player.OppositePlayer.InPlay.Creatures)
                                .Any();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var playerCreatures = Player.InPlay.Creatures;
            var enemyCreatures = Player.OppositePlayer.InPlay.Creatures;

            Player.OppositePlayer.Discard.AddAll(enemyCreatures);
            Player.Discard.AddAll(playerCreatures);
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class TransfigurationExam : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.InPlay.Creatures
                                .Concat(this.Player.OppositePlayer.InPlay.Creatures)
                                .Any();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var playerCreatures = this.Player.InPlay.Creatures;
            var enemyCreatures = this.Player.OppositePlayer.InPlay.Creatures;

            this.Player.OppositePlayer.Discard.AddAll(enemyCreatures);
            this.Player.Discard.AddAll(playerCreatures);
        }
    }
}

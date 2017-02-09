using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class CatchTheSnitch : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.InPlay.Matches.Any() || this.Player.OppositePlayer.InPlay.Matches.Any();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var match = (BaseMatch) this.Player.InPlay.Matches.Concat(this.Player.OppositePlayer.InPlay.Matches).Single();

            this.Player.Discard.Add(match);
            match.OnPlayerHasWonMatch(this.Player, this.Player.OppositePlayer);
        }
    }
}
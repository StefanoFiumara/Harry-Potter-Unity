using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class CatchTheSnitch : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.InPlay.Matches.Any() || Player.OppositePlayer.InPlay.Matches.Any();
        }

        //TODO: Test this
        protected override void SpellAction(List<BaseCard> targets)
        {
            var match = (BaseMatch) Player.InPlay.Matches.Concat(Player.OppositePlayer.InPlay.Matches).Single();

            Player.Discard.Add(match);
            match.OnPlayerHasWonMatch(Player, Player.OppositePlayer);
        }
    }
}
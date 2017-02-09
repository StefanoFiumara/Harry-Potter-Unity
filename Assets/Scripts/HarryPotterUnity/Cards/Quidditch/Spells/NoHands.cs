using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class NoHands : BaseSpell
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.InPlay.Matches.Any() || this.Player.OppositePlayer.InPlay.Matches.Any();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var topCard = this.Player.Deck.TakeTopCard();

            this.Player.Discard.Add(topCard);

            if (topCard is BaseLesson)
            {
                var match = (BaseMatch) this.Player.InPlay.Matches.Concat(this.Player.OppositePlayer.InPlay.Matches).Single();

                this.Player.Discard.Add(match);
                match.OnPlayerHasWonMatch(this.Player, this.Player.OppositePlayer);
            }
            else
            {
                this.Player.TakeDamage(this, 3);
            }
            
        }
    }
}
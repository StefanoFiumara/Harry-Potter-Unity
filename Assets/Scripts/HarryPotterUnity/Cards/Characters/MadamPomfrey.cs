using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Characters
{
    public class MadamPomfrey : BaseCharacter
    {
        private bool HasUsedAbility { get; set; }

        public override bool CanPerformInPlayAction()
        {
            return this.HasUsedAbility == false 
                && this.Player.CanUseActions()
                && this.Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            this.HasUsedAbility = true;

            var cards = this.Player.Discard.NonHealingCards.Take(12);

            this.Player.Deck.AddAll(cards);

            this.Player.Deck.Shuffle();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class PhilosophersStone : BaseItem
    {
        public override bool CanPerformInPlayAction()
        {
            return this.Player.CanUseActions(2) && this.Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            this.Player.Discard.Add(this);

            var allCards = this.Player.Discard.NonHealingCards;

            var lessons = allCards.Where(c => c.Type == Type.Lesson);
            var other = allCards.Where(c => c.Type != Type.Lesson);

            this.Player.InPlay.AddAll(lessons);
            this.Player.Deck.AddAll(other);

            this.Player.Deck.Shuffle();

            this.Player.UseActions(2);
        }
    }
}
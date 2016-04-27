using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class PhilosophersStone : BaseItem
    {
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions(2) && Player.IsLocalPlayer;
        }

        //TODO: Test This
        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var allCards = Player.Discard.NonHealingCards;

            var lessons = allCards.Where(c => c.Type == Type.Lesson);
            var other = allCards.Where(c => c.Type != Type.Lesson);
            
            Player.InPlay.AddAll(lessons);
            Player.Deck.AddAll(other);

            Player.Deck.Shuffle();

            Player.UseActions(2);
        }
    }
}
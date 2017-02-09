using System.Collections.Generic;
using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Charms.Items
{
    public class BrokenWand : ItemLessonProvider
    {
        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var card = this.Player.Deck.TakeTopCard();

            if (card is BaseLesson)
            {
                this.Player.InPlay.Add(card);
            }
            else
            {
                this.Player.Discard.Add(card);
            }

            this.Player.UseActions();
        }

        public override bool CanPerformInPlayAction()
        {
            return this.Player.CanUseActions() && this.Player.IsLocalPlayer;
        }
    }
}

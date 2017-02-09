using System.Collections.Generic;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class ItemLessonBook : ItemLessonProvider
    {
        public override bool CanPerformInPlayAction()
        {
            return this.Player.CanUseActions() && this.Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            this.Player.Discard.Add(this);

            this.Player.Deck.DrawCard();
            this.Player.Deck.DrawCard();
            this.Player.Deck.DrawCard();

            this.Player.UseActions();
        }
    }
}

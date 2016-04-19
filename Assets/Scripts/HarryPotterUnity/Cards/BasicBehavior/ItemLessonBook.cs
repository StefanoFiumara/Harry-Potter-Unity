using System.Collections.Generic;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class ItemLessonBook : ItemLessonProvider {
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            Player.Discard.Add(this);

            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();

            Player.UseActions();
        }
    }
}

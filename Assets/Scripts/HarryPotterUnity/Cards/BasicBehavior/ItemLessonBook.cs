namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class ItemLessonBook : ItemLessonProvider {
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions();
        }

        public override void OnSelectedAction()
        {
            Player.Discard.Add(this);

            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();

            Player.UseActions();
        }
    }
}

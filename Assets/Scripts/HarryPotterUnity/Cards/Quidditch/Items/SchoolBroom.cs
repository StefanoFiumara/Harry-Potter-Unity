using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    public class SchoolBroom : ItemLessonProvider
    {
        public override void OnEnterInPlayAction()
        {
            this.Player.Deck.DrawCard();
        }
    }
}
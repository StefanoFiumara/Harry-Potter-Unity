using HarryPotterUnity.Cards.BasicBehavior;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    [UsedImplicitly]
    public class SchoolBroom : ItemLessonProvider
    {
        public override void OnEnterInPlayAction()
        {
            Player.Deck.DrawCard();
        }
    }
}
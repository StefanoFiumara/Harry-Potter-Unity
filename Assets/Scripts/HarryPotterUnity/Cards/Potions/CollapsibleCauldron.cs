using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Potions
{
    public class CollapsibleCauldron : ItemLessonProvider
    {
        public override void OnExitInPlayAction()
        {
            Player.Hand.Add(this);
        }
    }
}

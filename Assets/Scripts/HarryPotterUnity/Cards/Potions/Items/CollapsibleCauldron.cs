using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Potions.Items
{
    public class CollapsibleCauldron : ItemLessonProvider
    {
        public override void OnExitInPlayAction()
        {
            this.Player.Hand.Add(this);
        }
    }
}

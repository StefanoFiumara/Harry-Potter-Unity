using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Potions.Items
{
    public class SelfStirringCauldron : ItemLessonProvider
    {
        public override void OnEnterInPlayAction()
        {
            this.Player.AddActions(1);
        }
    }
}

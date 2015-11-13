using HarryPotterUnity.Cards.BasicBehavior;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Potions
{
    [UsedImplicitly]
    public class SelfStirringCauldron : ItemLessonProvider
    {
        public override void OnEnterInPlayAction()
        {
            Player.AddActions(1);
        }
    }
}

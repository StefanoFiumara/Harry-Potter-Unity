using HarryPotterUnity.Cards.BasicBehavior;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Potions
{
    [UsedImplicitly]
    public class CollapsibleCauldron : ItemLessonProvider
    {
        public override void OnExitInPlayAction()
        {
            Player.Hand.Add(this);
        }
    }
}

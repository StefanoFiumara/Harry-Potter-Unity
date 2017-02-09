using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Characters
{
    public class HermioneGranger : BaseCharacter
    {
        public override bool CanPerformInPlayAction()
        {
            return this.Player.CanUseActions() && this.Player.Hand.Lessons.Count  >= 2 && this.Player.AmountLessonsInPlay >= 2 && this.Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var firstLesson = this.Player.Hand.Lessons.First();
            var secondLesson = this.Player.Hand.Lessons.Last();

            this.Player.InPlay.AddAll(new [] {firstLesson, secondLesson });
            this.Player.UseActions();
        }
    }
}
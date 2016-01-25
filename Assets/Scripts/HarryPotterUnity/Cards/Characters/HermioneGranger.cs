using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Characters
{
    [UsedImplicitly]
    public class HermioneGranger : BaseCharacter
    {
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && 
                   Player.Hand.Lessons.Count>= 2 &&
                   Player.AmountLessonsInPlay >= 2 &&
                   Player.IsLocalPlayer;
        }

        public override void OnSelectedAction()
        {
            var firstLesson = Player.Hand.Lessons.First();
            var secondLesson = Player.Hand.Lessons.Last();

            Player.InPlay.AddAll(new [] {firstLesson, secondLesson });
            Player.UseActions();
        }
    }
}
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
                   Player.Hand.Cards.Count(c => c.Type == Type.Lesson) >= 2 &&
                   Player.AmountLessonsInPlay >= 2;
        }

        public override void OnSelectedAction()
        {
            var firstLesson = Player.Hand.Cards.Find(c => c.Type == Type.Lesson);
            var secondLesson = Player.Hand.Cards.FindLast(c => c.Type == Type.Lesson);

            Player.InPlay.Add(firstLesson);
            Player.InPlay.Add(secondLesson);
            Player.Hand.RemoveAll(new[] {firstLesson, secondLesson});

            Player.UseActions();
        }
    }
}
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Potions.Locations
{
    public class PotionsDungeon : BaseLocation
    {
        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            var removeLessonFromPlayRequirements = GameManager.AllCards
                .Select(c => c.GetComponent<RemoveLessonFromPlayRequirement>())
                .Where(req => req != null && req.LessonType == LessonTypes.Potions);

            foreach (var requirement in removeLessonFromPlayRequirements)
            {
                requirement.ReturnToHand = true;
            }


        }

        public override void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            var removeLessonFromPlayRequirements = GameManager.AllCards
                .Select(c => c.GetComponent<RemoveLessonFromPlayRequirement>())
                .Where(req => req != null && req.LessonType == LessonTypes.Potions);

            foreach (var requirement in removeLessonFromPlayRequirements)
            {
                requirement.Reset();
            }
        }
    }
}
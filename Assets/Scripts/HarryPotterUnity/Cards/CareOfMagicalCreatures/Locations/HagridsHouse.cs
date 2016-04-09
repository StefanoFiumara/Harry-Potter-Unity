using System.Linq;
using System.Net.Cache;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Locations
{
    public class HagridsHouse : BaseLocation
    {
        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            var removeLessonFromPlayRequirements = GameManager.AllCards
                .Select(c => c.GetComponent<RemoveLessonFromPlayRequirement>())
                .Where(req => req != null && req.LessonType == LessonTypes.Creatures);

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
                .Where(req => req != null && req.LessonType == LessonTypes.Creatures);

            foreach (var requirement in removeLessonFromPlayRequirements)
            {
                requirement.Reset();
            }
        }
    }
}
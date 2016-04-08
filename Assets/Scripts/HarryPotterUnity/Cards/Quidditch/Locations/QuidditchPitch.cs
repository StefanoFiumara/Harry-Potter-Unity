using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Locations
{
    public class QuidditchPitch : BaseLocation
    {
        public override void OnEnterInPlayAction()
        {
            var quidditchSpellsLessonRequirements =
                GameManager.AllCards.Where(c => c.Type == Type.Spell && c.Classification == ClassificationTypes.Quidditch)
                .Select(spell => spell.GetComponent<LessonRequirement>())
                .ToList();

            foreach (var requirement in quidditchSpellsLessonRequirements)
            {
                requirement.AmountRequired -= 2;

                if (requirement.AmountRequired < 1)
                {
                    requirement.AmountRequired = 1;
                }
            }
        }

        //TODO: Test this
        public override void OnExitInPlayAction()
        {
            var lessonRequirements =
                GameManager.AllCards.Where(c => c.Type == Type.Spell && c.Classification == ClassificationTypes.Quidditch)
                .Select(spell => spell.GetComponent<LessonRequirement>())
                .ToList();

            foreach (var req in lessonRequirements)
            {
                req.ResetRequirement();
            }
        }
    }
}
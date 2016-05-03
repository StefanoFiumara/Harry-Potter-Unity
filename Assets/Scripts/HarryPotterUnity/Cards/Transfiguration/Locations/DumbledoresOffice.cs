using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Transfiguration.Locations
{
    public class DumbledoresOffice : BaseLocation
    {
        //TODO: Test this
        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            var itemLessonRequirements =
                GameManager.AllCards.Where(card => card.Type == Type.Item)
                .Select(card => card.GetComponent<LessonRequirement>())
                .ToList();

            foreach (var requirement in itemLessonRequirements)
            {
                requirement.AmountRequired -= 3;

                if (requirement.AmountRequired < 1)
                {
                    requirement.AmountRequired = 1;
                }
            }
        }

        public override void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            var itemLessonRequirements =
                GameManager.AllCards.Where(card => card.Type == Type.Item)
                .Select(card => card.GetComponent<LessonRequirement>())
                .ToList();

            foreach (var req in itemLessonRequirements)
            {
                req.ResetRequirement();
            }
        }
    }
}
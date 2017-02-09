using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Characters
{
    public class AlbusDumbledore : BaseCharacter
    {
        public override void OnEnterInPlayAction()
        {
            var lessonRequirements = this.Player.AllCards.Where(c => c.Type == Type.Spell)
                .Select(spell => spell.GetComponent<LessonRequirement>())
                .Where(req => req.AmountRequired >= 6);

            foreach (var requirement in lessonRequirements)
            {
                requirement.AmountRequired -= 2;
            }
        }

        public override void OnExitInPlayAction()
        {
            var lessonRequirements = this.Player.AllCards.Where(c => c.Type == Type.Spell)
                .Select(spell => spell.GetComponent<LessonRequirement>())
                .Where(req => req.AmountRequired >= 6);

            foreach (var requirement in lessonRequirements)
            {
                requirement.ResetRequirement();
            }
        }
    }
}
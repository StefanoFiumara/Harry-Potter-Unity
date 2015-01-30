namespace Assets.Scripts.HarryPotterUnity.Cards.Creatures
{
    public class CreatureRequiresDiscard : GenericCreature, IPersistentCard {

        public int LessonsToDiscard;
        protected override bool MeetsAdditionalRequirements()
        {
            return Player.InPlay.GetLessonsInPlay()
                .FindAll(card => ((Lesson) card).LessonType == Lesson.LessonTypes.Creatures)
                .Count >= LessonsToDiscard;
        }

        public new void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            //TODO: Remove lessons here
        }
    }
}

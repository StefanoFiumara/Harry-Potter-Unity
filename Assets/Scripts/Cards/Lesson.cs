namespace Assets.Scripts.Cards
{
    public class Lesson : GenericCard, IPersistentCard {

        public enum LessonTypes
        {
            Creatures = 0, Charms, Transfiguration, Potions, Quidditch
        }

        public LessonTypes LessonType;

        public void OnMouseUp()
        {
            if (State != CardStates.InHand) return;

            if (!_Player.CanUseAction()) return;

            _Player._Hand.Remove(this);
            _Player._InPlay.Add(this);
            _Player.UseAction();
        }

        public void OnEnterInPlayAction()
        {
            if (!_Player.LessonTypesInPlay.Contains(LessonType))
            {
                _Player.LessonTypesInPlay.Add(LessonType);
            }
        
            _Player.AmountLessonsInPlay++;

            State = CardStates.InPlay;
        }

        public void OnExitInPlayAction()
        {
            _Player.AmountLessonsInPlay--;
            _Player.UpdateLessonTypesInPlay();
        }

        //Lesson Cards don't implement these methods
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
    }
}

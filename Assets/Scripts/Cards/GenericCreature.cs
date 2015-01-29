namespace Assets.Scripts.Cards
{
    public class GenericCreature : GenericCard, IPersistentCard {

        public Lesson.LessonTypes CostType;

        public int CostAmount;

        public int DamagePerTurn;
        public int Health;

        public void OnMouseUp()
        {
            if (State != CardStates.InHand) return;

            if (!Player.CanUseAction()) return;

            if (Player.AmountLessonsInPlay < CostAmount || !Player.LessonTypesInPlay.Contains(CostType)) return;

            if (!MeetsAdditionalRequirements()) return;

            Player.Hand.Remove(this);
            Player.InPlay.Add(this);
            Player.UseAction();
        }

        public void OnEnterInPlayAction()
        {
            Player.CreaturesInPlay++;
            Player.DamagePerTurn += DamagePerTurn;

            State = CardStates.InPlay;
        }

        public void OnExitInPlayAction()
        {
            Player.CreaturesInPlay--;
            Player.DamagePerTurn -= DamagePerTurn;
        }

        protected virtual bool MeetsAdditionalRequirements()
        {
            return true;
        }

        //Generic Creatures don't do anything special on these actions
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
  
    }
}

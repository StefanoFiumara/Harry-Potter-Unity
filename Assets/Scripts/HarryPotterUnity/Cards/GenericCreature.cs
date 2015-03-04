namespace HarryPotterUnity.Cards
{
    public class GenericCreature : GenericCard, IPersistentCard {

        public Lesson.LessonTypes CostType;

        public int CostAmount;

        public int DamagePerTurn;
        public int Health;

        public override void OnClickAction()
        {
            Player.Hand.Remove(this);
            Player.InPlay.Add(this);
        }

        public override bool MeetsAdditionalPlayRequirements()
        {
            return Player.AmountLessonsInPlay > CostAmount && 
                  Player.LessonTypesInPlay.Contains(CostType) &&
                  MeetsAdditionalCreatureRequirements();
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

        protected virtual bool MeetsAdditionalCreatureRequirements()
        {
            return true;
        }

        //Generic Creatures don't do anything special on these actions
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
  
    }
}

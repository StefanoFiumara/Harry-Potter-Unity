using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    public class GenericCreature : GenericCard, IPersistentCard {

        [UsedImplicitly, SerializeField]
        private Lesson.LessonTypes _costType;

        [UsedImplicitly, SerializeField]
        private int _costAmount;

        [UsedImplicitly, SerializeField]
        private int _damagePerTurn;

        [UsedImplicitly]
        public int Health;

        protected override void OnClickAction()
        {
            Player.InPlay.Add(this);
            Player.Hand.Remove(this);
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.AmountLessonsInPlay >= _costAmount && 
                  Player.LessonTypesInPlay.Contains(_costType) &&
                  MeetsAdditionalCreatureRequirements();
        }

        public void OnEnterInPlayAction()
        {
            Player.CreaturesInPlay++;
            Player.DamagePerTurn += _damagePerTurn;

            State = CardStates.InPlay;
        }

        public void OnExitInPlayAction()
        {
            Player.CreaturesInPlay--;
            Player.DamagePerTurn -= _damagePerTurn;
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

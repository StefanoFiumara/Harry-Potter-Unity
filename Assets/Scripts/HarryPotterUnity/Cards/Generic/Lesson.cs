using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    [UsedImplicitly]
    public class Lesson : GenericCard, IPersistentCard {

        public enum LessonTypes
        {
            Creatures = 0, Charms, Transfiguration, Potions, Quidditch
        }

        [UsedImplicitly, SerializeField]
        private LessonTypes _lessonType;

        public LessonTypes LessonType {
            get { return _lessonType; }
        }

        protected override void OnClickAction(List<GenericCard> targets)
        {
            Player.InPlay.Add(this);
            Player.Hand.Remove(this);
        }

        public void OnEnterInPlayAction()
        {
            if (!Player.LessonTypesInPlay.Contains(_lessonType))
            {
                Player.LessonTypesInPlay.Add(_lessonType);
            }
        
            Player.AmountLessonsInPlay++;

            State = CardStates.InPlay;
        }

        public void OnExitInPlayAction()
        {
            Player.AmountLessonsInPlay--;
            Player.UpdateLessonTypesInPlay();
        }

        //Lesson Cards don't implement these methods
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
    }
}

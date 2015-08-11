using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    [UsedImplicitly]
    public class Lesson : GenericCard, IPersistentCard, ILessonProvider {

        public enum LessonTypes
        {
            Creatures = 0, Charms, Transfiguration, Potions, Quidditch
        }
        
        [Header("Lesson Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private LessonTypes _lessonType;

        public LessonTypes LessonType {
            get { return _lessonType; }
        }

        public int AmountLessonsProvided { get { return 1; } }

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
        
            Player.AmountLessonsInPlay += AmountLessonsProvided;

            State = CardStates.InPlay;
        }

        public void OnExitInPlayAction()
        {
            Player.AmountLessonsInPlay -= AmountLessonsProvided;
            Player.UpdateLessonTypesInPlay();
        }

        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
    }
}

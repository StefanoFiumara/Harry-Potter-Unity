using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic {

    [UsedImplicitly]
    public class LessonProvider : GenericCard, IPersistentCard, ILessonProvider
    {
        

        #region Inspector Settings
        [Header("Lesson Provider Settings")]
        [SerializeField, UsedImplicitly] private LessonTypes _lessonType;

        [SerializeField, UsedImplicitly] private int _amountLessonsProvided;
        #endregion

        #region Properties
        public LessonTypes LessonType
        {
            get { return _lessonType; }
        }

        public int AmountLessonsProvided
        {
            get { return _amountLessonsProvided; }
        }
        #endregion

        protected override void OnClickAction(List<GenericCard> targets)
        {
            Player.InPlay.Add(this);
            Player.Hand.Remove(this);
        }
    
        public void OnEnterInPlayAction()
        {

            if (!Player.LessonTypesInPlay.Contains(LessonType))
            {
                Player.LessonTypesInPlay.Add(LessonType);
            }

            Player.AmountLessonsInPlay += AmountLessonsProvided;

            State = State.InPlay;
        }

        public void OnExitInPlayAction()
        {
            Player.AmountLessonsInPlay -= AmountLessonsProvided;
            Player.UpdateLessonTypesInPlay();
        }

        public bool CanPerformInPlayAction() { return false; }

        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
    }
}

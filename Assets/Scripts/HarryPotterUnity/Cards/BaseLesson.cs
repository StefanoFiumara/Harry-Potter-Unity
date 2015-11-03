using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards {

    [UsedImplicitly]
    public class BaseLesson : BaseCard, IPersistentCard, ILessonProvider
    {
        #region Inspector Settings
        [Header("Lesson Settings")]
        [SerializeField, UsedImplicitly]
        private LessonTypes _lessonType;
        #endregion
        
        public LessonTypes LessonType { get { return _lessonType; } }
        public int AmountLessonsProvided { get { return 1; } }
        
        protected override Type GetCardType() { return Type.Lesson; }

        public bool CanPerformInPlayAction() { return false; }

        public void OnEnterInPlayAction() { }
        public void OnExitInPlayAction() { }
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
    }
}

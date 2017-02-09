using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards {

    public sealed class BaseLesson : BaseCard, IPersistentCard, ILessonProvider
    {
        #region Inspector Settings
        [Header("Lesson Settings")]
        [SerializeField, UsedImplicitly]
        private LessonTypes _lessonType;
        #endregion
        
        public LessonTypes LessonType { get { return this._lessonType; } }
        public int AmountLessonsProvided { get; set; }

        protected override void Start()
        {
            base.Start();
            this.AmountLessonsProvided = 1;
        }

        protected override Type GetCardType() { return Type.Lesson; }

        public bool CanPerformInPlayAction() { return false; }
        public void OnInPlayAction(List<BaseCard> targets = null) { }

        public void OnEnterInPlayAction() { }

        public void OnExitInPlayAction()
        {
            //Reset Amount Provided, cards like wand shop may alter this value during play
            this.AmountLessonsProvided = 1;
        }

        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        
    }
}

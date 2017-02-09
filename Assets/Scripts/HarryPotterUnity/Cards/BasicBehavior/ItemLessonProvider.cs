using System;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class ItemLessonProvider : BaseItem, ILessonProvider
    {
        #region Inspector Settings
        [Header("Lesson Provider Settings")]
        [SerializeField, UsedImplicitly]
        private LessonTypes _lessonType;
        [SerializeField, UsedImplicitly]
        private int _amountLessonsProvided;
        #endregion

        public LessonTypes LessonType { get {return this._lessonType; } }

        public int AmountLessonsProvided
        {
            get
            {
                return this._amountLessonsProvided;
            }
            set
            {
                throw new InvalidOperationException("Cannot change the amount of lessons provided by ItemLessonProvider");
            }
        }
    }
}

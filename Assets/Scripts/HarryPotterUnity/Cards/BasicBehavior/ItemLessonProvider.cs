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

        public LessonTypes LessonType { get {return _lessonType; } }
        public int AmountLessonsProvided { get {return _amountLessonsProvided;} }
    }
}

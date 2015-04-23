using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    [UsedImplicitly]
    public class LessonRequirement : MonoBehaviour, ICardPlayRequirement
    {

        [SerializeField, UsedImplicitly]
        private int _amountRequired;

        [SerializeField, UsedImplicitly]
        private Lesson.LessonTypes _lessonType;

        private Player _player;

        [UsedImplicitly]
        void Start()
        {
            _player = GetComponent<GenericCard>().Player;
        }
        public bool MeetsRequirement()
        {
            return _player.AmountLessonsInPlay >= _amountRequired &&
                   _player.LessonTypesInPlay.Contains(_lessonType);
        }

        public void OnRequirementMet() { }
    }
}

using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    [UsedImplicitly]
    public class LessonDiscardRequirement : MonoBehaviour, ICardPlayRequirement
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
            return _player.InPlay.GetAmountOfLessonsOfType( _lessonType ) >= _amountRequired;
        }

        public void OnRequirementMet()
        {
            for (int i = 0; i < _amountRequired; i++)
            {
                var lesson = _player.InPlay.GetLessonOfType( _lessonType );

                _player.InPlay.Remove(lesson);
                _player.Discard.Add(lesson);
            }
        }
    }
}

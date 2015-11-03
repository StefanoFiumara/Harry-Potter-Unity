using System.Linq;
using HarryPotterUnity.Enums;
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
        private LessonTypes _lessonType;

        private Player _player;

        [UsedImplicitly]
        void Start()
        {
            _player = GetComponent<BaseCard>().Player;
        }
        public bool MeetsRequirement()
        {
            return _player.InPlay.GetAmountOfLessonsOfType( _lessonType ) >= _amountRequired;
        }

        public void OnRequirementMet()
        {
            var lessons = _player.InPlay.GetLessonsOfType(_lessonType, _amountRequired).ToList();

            _player.Discard.AddAll(lessons);
            _player.InPlay.RemoveAll(lessons);
        }
    }
}

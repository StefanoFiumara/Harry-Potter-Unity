using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class RemoveLessonFromPlayRequirement : MonoBehaviour, ICardPlayRequirement
    {

        [SerializeField, UsedImplicitly]
        private int _amountRequired;

        [SerializeField, UsedImplicitly]
        private LessonTypes _lessonType;

        [SerializeField, UsedImplicitly]
        private bool _returnToHand;

        private Player _player;

        private void Start()
        {
            _player = GetComponent<BaseCard>().Player;
        }
        public bool MeetsRequirement()
        {
            return _player.InPlay
                .LessonsOfType(_lessonType).Count() >= _amountRequired;
        }

        public void OnRequirementMet()
        {
            var lessons = _player.InPlay.LessonsOfType(_lessonType)
                .Take(_amountRequired).ToList();

            if (_returnToHand)
            {
                _player.Hand.AddAll(lessons);
            }
            else
            {
                _player.Discard.AddAll(lessons);
            }
        }
    }
}

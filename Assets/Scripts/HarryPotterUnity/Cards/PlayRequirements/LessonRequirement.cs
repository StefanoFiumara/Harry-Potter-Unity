using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class LessonRequirement : MonoBehaviour, ICardPlayRequirement
    {

        [SerializeField]
        private int _amountRequired;

        [SerializeField]
        private LessonTypes _lessonType;

        private Player _player;

        public int AmountRequired
        {
            get { return _amountRequired; }
            set { _amountRequired = value; }
        }

        public LessonTypes LessonType
        {
            get { return _lessonType; }
            set { _lessonType = value; }
        }

        private void Start()
        {
            _player = GetComponent<BaseCard>().Player;
        }
        public bool MeetsRequirement()
        {
            return _player.AmountLessonsInPlay >= _amountRequired &&
                   _player.LessonTypesInPlay.Contains(LessonType);
        }

        public void OnRequirementMet() { }
    }
}

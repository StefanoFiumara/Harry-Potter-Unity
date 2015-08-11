using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic.PlayRequirements
{
    public class CardInPlayProvidesLessonsRequirement : MonoBehaviour, ICardPlayRequirement
    {

        private Player _player;

        [UsedImplicitly, SerializeField]
        private LessonTypes _lessonType;

        [UsedImplicitly, SerializeField]
        private int _amountProvided;
    
        [UsedImplicitly]
        void Start()
        {
            _player = GetComponent<GenericCard>().Player;
        }

        public bool MeetsRequirement()
        {
            return _player.InPlay.Cards.Exists(card => card is ILessonProvider &&
                                                       ((ILessonProvider) card).AmountLessonsProvided >= _amountProvided &&
                                                       ((ILessonProvider) card).LessonType == _lessonType);
        }

        public void OnRequirementMet() { }
    }
}

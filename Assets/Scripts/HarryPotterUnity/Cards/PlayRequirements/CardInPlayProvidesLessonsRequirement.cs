using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class CardInPlayProvidesLessonsRequirement : MonoBehaviour, ICardPlayRequirement
    {
        private Player _player;

        [UsedImplicitly, SerializeField]
        private LessonTypes _lessonType;

        [UsedImplicitly, SerializeField]
        private int _amountProvided;
    
        private void Start()
        {
            _player = GetComponent<BaseCard>().Player;
        }

        public bool MeetsRequirement()
        {
            return _player.InPlay.Cards
                .OfType<ILessonProvider>()
                .Count(c => c.LessonType == LessonTypes.Charms) >= _amountProvided;
        }

        public void OnRequirementMet() { }
    }
}

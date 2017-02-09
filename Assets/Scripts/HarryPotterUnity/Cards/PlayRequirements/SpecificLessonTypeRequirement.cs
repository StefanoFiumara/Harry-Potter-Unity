using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class SpecificLessonTypeRequirement : MonoBehaviour, ICardPlayRequirement
    {
        private Player _player;

        [UsedImplicitly, SerializeField]
        private LessonTypes _lessonType;

        [UsedImplicitly, SerializeField]
        private int _amountProvided;
    
        private void Start()
        {
            this._player = this.GetComponent<BaseCard>().Player;
        }

        public bool MeetsRequirement()
        {
            return this._player.InPlay.Cards
                .OfType<ILessonProvider>()
                .Count(c => c.LessonType == this._lessonType) >= this._amountProvided;
        }

        public void OnRequirementMet() { }
    }
}

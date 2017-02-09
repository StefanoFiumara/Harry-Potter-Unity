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

        public bool ReturnToHand { get; set; }

        public LessonTypes LessonType { get { return this._lessonType; } }

        private void Start()
        {
            this._player = this.GetComponent<BaseCard>().Player;
            this.ReturnToHand = this._returnToHand;
        }
        public bool MeetsRequirement()
        {
            return this._player.InPlay
                .LessonsOfType(this._lessonType).Count() >= this._amountRequired;
        }

        public void OnRequirementMet()
        {
            var lessons = this._player.InPlay.LessonsOfType(this._lessonType)
                .Take(this._amountRequired).ToList();

            if (this.ReturnToHand)
            {
                this._player.Hand.AddAll(lessons);
            }
            else
            {
                this._player.Discard.AddAll(lessons);
            }
        }

        public void Reset()
        {
            this.ReturnToHand = this._returnToHand;
        }
    }
}

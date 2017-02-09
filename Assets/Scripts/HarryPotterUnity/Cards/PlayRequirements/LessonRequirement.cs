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
            get { return this._amountRequired; }
            set { this._amountRequired = value; }
        }

        public LessonTypes LessonType
        {
            get { return this._lessonType; }
            set { this._lessonType = value; }
        }

        private int _originalAmountRequired; //Some cards may change how many lessons are required to play a card, this variable holds the "printed" amount.

        private void Start()
        {
            this._player = this.GetComponent<BaseCard>().Player;
            this._originalAmountRequired = this._amountRequired;
        }
        public bool MeetsRequirement()
        {
            return this._player.AmountLessonsInPlay >= this._amountRequired && this._player.LessonTypesInPlay.Contains(this.LessonType);
        }

        public void OnRequirementMet() { }

        public void ResetRequirement()
        {
            this.AmountRequired = this._originalAmountRequired;
        }
    }
}

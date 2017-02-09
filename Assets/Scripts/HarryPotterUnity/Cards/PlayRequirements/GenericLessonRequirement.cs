using HarryPotterUnity.Game;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class GenericLessonRequirement : MonoBehaviour, ICardPlayRequirement
    {
        [SerializeField]
        private int _amountRequired;

        private Player _player;

        public int AmountRequired
        {
            get { return this._amountRequired; }
            set { this._amountRequired = value; }
        }

        private void Start()
        {
            this._player = this.GetComponent<BaseCard>().Player;
        }

        public bool MeetsRequirement()
        {
            return this._player.AmountLessonsInPlay >= this._amountRequired;
        }

        public void OnRequirementMet()
        {
            
        }
    }
}
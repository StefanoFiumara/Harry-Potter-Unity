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
            get { return _amountRequired; }
            set { _amountRequired = value; }
        }

        private void Start()
        {
            _player = GetComponent<BaseCard>().Player;
        }

        public bool MeetsRequirement()
        {
            return _player.AmountLessonsInPlay >= _amountRequired;
        }

        public void OnRequirementMet()
        {
            
        }
    }
}
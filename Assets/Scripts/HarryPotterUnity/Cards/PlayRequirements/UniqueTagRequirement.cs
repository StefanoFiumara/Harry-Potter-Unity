using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class UniqueTagRequirement : MonoBehaviour, ICardPlayRequirement
    {
        private Player _player;

        [SerializeField, UsedImplicitly]
        private Tag _tag;

        private void Start()
        {
            _player = GetComponent<BaseCard>().Player;
        }
        public bool MeetsRequirement()
        {
            return true;
        }

        public void OnRequirementMet()
        {
            if (!_player.InPlay.Cards.Exists(c => c.HasTag(_tag))) return;

            var card =_player.InPlay.Cards.Find(c => c.HasTag(_tag));

            _player.Discard.Add(card);
        }
    }
}

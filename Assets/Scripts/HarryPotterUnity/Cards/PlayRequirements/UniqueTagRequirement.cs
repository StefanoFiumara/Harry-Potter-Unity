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
            this._player = this.GetComponent<BaseCard>().Player;
        }
        public bool MeetsRequirement()
        {
            return true;
        }

        public void OnRequirementMet()
        {
            if (!this._player.InPlay.Cards.Exists(c => c.HasTag(this._tag))) return;

            var card = this._player.InPlay.Cards.Find(c => c.HasTag(this._tag));

            this._player.Discard.Add(card);
        }
    }
}

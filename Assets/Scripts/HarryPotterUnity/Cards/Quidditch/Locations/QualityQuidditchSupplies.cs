using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Locations
{
    public class QualityQuidditchSupplies : BaseLocation
    {
        public override bool CanPerformInPlayAction()
        {
            var player = this.Player.IsLocalPlayer ? this.Player : this.Player.OppositePlayer;

            return player.CanUseActions();
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var player = this.Player.CanUseActions() ? this.Player : this.Player.OppositePlayer;

            var items = player.Deck.Items.Where(c => c.Classification == ClassificationTypes.Quidditch).ToList();

            if (items.Any())
            {
                var selectedItem = items.Skip(Random.Range(0, items.Count)).First();
                player.Hand.Add(selectedItem);
            }

            player.Deck.Shuffle();
            player.UseActions();
        }
    }
}
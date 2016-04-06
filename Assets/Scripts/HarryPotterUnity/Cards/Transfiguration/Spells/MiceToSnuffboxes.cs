using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class MiceToSnuffboxes : BaseSpell
    {
        public override List<BaseCard> GetValidTargets()
        {
            var validCards = Player.InPlay.Creatures
                .Concat(Player.OppositePlayer.InPlay.Creatures)
                .ToList();

            return validCards;
        }

        protected override void SpellAction(List<BaseCard> selectedCards)
        {
            //Check if the cards belong to the same player and do and AddAll to prevent animation bugs
            bool localPlayer = selectedCards.First().Player.IsLocalPlayer;
            if (selectedCards.All(c => c.Player.IsLocalPlayer == localPlayer))
            {
                selectedCards.First().Player.Hand.AddAll(selectedCards);

            }
            else
            {
                foreach (var card in selectedCards)
                {
                    card.Player.Hand.Add(card, preview: false, adjustSpacing: true);
                }
            }
        }
    }
}

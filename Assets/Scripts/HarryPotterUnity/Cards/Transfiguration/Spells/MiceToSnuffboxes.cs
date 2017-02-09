using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class MiceToSnuffboxes : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            var validCards = this.Player.InPlay.Creatures
                .Concat(this.Player.OppositePlayer.InPlay.Creatures)
                .ToList();

            return validCards;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            //Check if the cards belong to the same player and do and AddAll to prevent animation bugs
            bool localPlayer = targets.First().Player.IsLocalPlayer;
            if (targets.All(c => c.Player.IsLocalPlayer == localPlayer))
            {
                targets.First().Player.Hand.AddAll(targets);

            }
            else
            {
                foreach (var card in targets)
                {
                    card.Player.Hand.Add(card, preview: false, adjustSpacing: true);
                }
            }
        }
    }
}

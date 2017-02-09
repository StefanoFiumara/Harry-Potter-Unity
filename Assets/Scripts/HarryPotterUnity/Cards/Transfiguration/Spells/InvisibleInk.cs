using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [RequireComponent(typeof(InputRequirement))]
    public class InvisibleInk : BaseSpell
    {
        public override List<BaseCard> GetFromHandActionTargets()
        {
            return this.Player.InPlay.Lessons.Concat(this.Player.OppositePlayer.InPlay.Lessons).ToList();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            bool localPlayer = targets.First().Player.IsLocalPlayer;
            if (targets.All(c => c.Player.IsLocalPlayer == localPlayer))
            {
                targets.First().Player.Discard.AddAll(targets);
            }
            else
            {
                foreach (var card in targets)
                {
                    card.Player.Discard.Add(card);
                }
            }
        }
    }
}
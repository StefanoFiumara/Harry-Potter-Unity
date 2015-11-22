using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    [RequireComponent(typeof(InputRequirement))]
    public class Smash : BaseSpell 
    {
        public override List<BaseCard> GetValidTargets()
        {
            return Player.OppositePlayer.InPlay.CardsExceptStartingCharacter.Where(c => c.Type == Type.Item).ToList();
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();
            Player.OppositePlayer.Discard.Add(target);
        }
    }
}

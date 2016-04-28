using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.PlayRequirements;
using UnityEngine;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Items
{
    [RequireComponent(typeof(InputRequirement))]
    public class GuideToHouseholdPests : ItemLessonProvider
    {
        public override List<BaseCard> GetInPlayActionTargets()
        {
            return Player.OppositePlayer.InPlay.Creatures.Concat(Player.InPlay.Creatures).ToList();
        }

        public override bool CanPerformInPlayAction()
        {
            return Player.IsLocalPlayer
                   && Player.CanUseActions();
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            if (targets != null)
            {
                Player.Discard.Add(this);

                var target = targets.First();
                ((BaseCreature)target).TakeDamage(4);

                Player.UseActions();
            }
        }
    }
}
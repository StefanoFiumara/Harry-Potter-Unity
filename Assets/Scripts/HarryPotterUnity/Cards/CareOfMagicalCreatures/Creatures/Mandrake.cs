using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class Mandrake : BaseCreature
    {
        public override bool CanPerformInPlayAction()
        {
            return Player.Discard.Characters.Any(c => c.HasTag(Tag.Healing) == false);
        }

        //TODO: Test this
        public override void OnSelectedAction(List<BaseCard> targets = null)
        {
            Player.Discard.Add(this);

            var character = Player.Discard.Characters
                .Where(c => c.HasTag(Tag.Healing))
                .Skip(Random.Range(0, Player.Discard.Characters.Count))
                .First();

            Player.Hand.Add(character);
        }
    }
}
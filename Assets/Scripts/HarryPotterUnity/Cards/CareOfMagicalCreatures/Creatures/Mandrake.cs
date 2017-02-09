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
            return this.Player.Discard.Characters.Any(c => c.HasTag(Tag.Healing) == false) && this.Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            this.Player.Discard.Add(this);

            var possibleCharacters = this.Player.Discard.Characters
                .Where(c => c.HasTag(Tag.Healing) == false).ToList();

            var character = possibleCharacters
                .Skip(Random.Range(0, possibleCharacters.Count))
                .First();

            this.Player.Hand.Add(character);
        }
    }
}
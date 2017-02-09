using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class FlooPowder : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            var location = this.Player.Deck.Locations.Skip(Random.Range(0, this.Player.Deck.Locations.Count)).FirstOrDefault();

            if (location != null)
            {
                this.Player.Hand.Add(location);
                this.Player.Deck.Shuffle();
            }
        }
    }
}
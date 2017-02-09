using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class Fumos : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            this.DamageAmount = 2;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var allCreatures = this.Player.InPlay.Creatures.Concat(this.Player.OppositePlayer.InPlay.Creatures);

            foreach (var creature in allCreatures)
            {
                ((BaseCreature)creature).TakeDamage(this.DamageAmount);
            }
        }
    }
}
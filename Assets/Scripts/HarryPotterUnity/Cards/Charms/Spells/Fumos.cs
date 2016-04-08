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
            DamageAmount = 2;
        }

        //TODO: test this
        protected override void SpellAction(List<BaseCard> targets)
        {
            var allCreatures = Player.InPlay.Creatures.Concat(Player.OppositePlayer.InPlay.Creatures);

            foreach (var creature in allCreatures)
            {
                ((BaseCreature)creature).TakeDamage(DamageAmount);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class PowerPlay : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            this.DamageAmount = 7;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            this.Player.OppositePlayer.TakeDamage(this, this.DamageAmount);

            this.DamageAmount = 7;
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.InPlay.Matches.Any() || this.Player.OppositePlayer.InPlay.Matches.Any();
        }
    }
}
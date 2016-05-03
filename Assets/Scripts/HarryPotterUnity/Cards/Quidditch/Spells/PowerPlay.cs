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
            DamageAmount = 7;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);

            DamageAmount = 7;
        }

        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.InPlay.Matches.Any() || Player.OppositePlayer.InPlay.Matches.Any();
        }
    }
}
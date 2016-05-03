using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class PullingUp : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            DamageAmount = 2;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);

            Player.Deck.DrawCard();
            Player.Deck.DrawCard();

            DamageAmount = 2;
        }
    }
}
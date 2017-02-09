using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class LoopTheLoops : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        protected override void Start()
        {
            base.Start();
            this.DamageAmount = 4;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            this.Player.OppositePlayer.TakeDamage(this, this.DamageAmount);

            this.Player.Deck.DrawCard();
            this.Player.Deck.DrawCard();
            this.Player.Deck.DrawCard();
            this.Player.Deck.DrawCard();

            this.DamageAmount = 4;
        }
    }
}
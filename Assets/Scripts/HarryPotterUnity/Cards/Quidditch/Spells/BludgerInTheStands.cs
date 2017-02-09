using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class BludgerInTheStands : BaseSpell, IDamageSpell
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

            var item = this.Player.OppositePlayer.InPlay.Items.Skip(Random.Range(0, this.Player.OppositePlayer.InPlay.Items.Count))
                    .First();

            this.Player.OppositePlayer.Discard.Add(item);

            this.DamageAmount = 4;
        }
        
    }
}
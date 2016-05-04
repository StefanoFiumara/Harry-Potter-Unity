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
            DamageAmount = 4;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);

            var item =
                Player.OppositePlayer.InPlay.Items.Skip(Random.Range(0, Player.OppositePlayer.InPlay.Items.Count))
                    .First();

            Player.OppositePlayer.Discard.Add(item);

            DamageAmount = 4;
        }
        
    }
}
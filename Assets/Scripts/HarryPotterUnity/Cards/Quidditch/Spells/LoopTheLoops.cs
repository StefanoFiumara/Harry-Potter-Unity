using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    public class LoopTheLoops : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get { return 4; } }

        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(this, DamageAmount);

            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
        }
    }
}
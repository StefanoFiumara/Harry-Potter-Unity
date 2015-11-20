using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    [UsedImplicitly]
    public class LoopTheLoops : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            Player.OppositePlayer.TakeDamage(4);

            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
        }
    }
}
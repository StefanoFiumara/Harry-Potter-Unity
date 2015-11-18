using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    [UsedImplicitly]
    class HealingSpell : BaseSpell
    {

        [Header("Healing Spell Settings")]
        [SerializeField, UsedImplicitly]
        private int _healingAmount;

        [SerializeField, UsedImplicitly]
        private bool _shuffleDeckAfterHeal;

        protected override void SpellAction(List<BaseCard> targets)
        {
            var cards = Player.Discard.GetHealableCards(_healingAmount);
            
            Player.Deck.AddAll(cards);
            if (_shuffleDeckAfterHeal)
            {
                Player.Deck.Shuffle();
            }
        }
    }
}

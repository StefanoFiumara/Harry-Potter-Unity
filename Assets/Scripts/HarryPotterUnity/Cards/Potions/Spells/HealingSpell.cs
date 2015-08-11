using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    [UsedImplicitly]
    class HealingSpell : GenericSpell
    {

        [Header("Healing Spell Settings")]
        [SerializeField, UsedImplicitly]
        private int _healingAmount;

        [SerializeField, UsedImplicitly]
        private bool _shuffleDeckAfterHeal;

        protected override void SpellAction(List<GenericCard> targets)
        {
            var cards = Player.Discard.GetCards(card => !card.Tags.Contains(Tag.Healing)).Take(_healingAmount).ToList();
            
            Player.Discard.RemoveAll(cards);
            Player.Deck.AddAll(cards);
            if (_shuffleDeckAfterHeal)
            {
                Player.Deck.Shuffle();
            }
        }
    }
}

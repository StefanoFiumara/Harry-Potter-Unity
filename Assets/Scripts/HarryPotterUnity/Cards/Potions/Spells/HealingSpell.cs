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

        [SerializeField, UsedImplicitly]
        private int _healingAmount;

        [SerializeField, UsedImplicitly]
        private bool _shuffleDeck;

        protected override void SpellAction(List<GenericCard> targets)
        {
            var cards = Player.Discard.GetCards(card => !card.Tags.Contains(Tag.Healing)).Take(_healingAmount).ToList();
            /*
            foreach (var card in cards)
            {
                Player.Discard.Remove(card);
                Player.Deck.Add(card);
            }
            */

            Player.Discard.RemoveAll(cards);
            Player.Deck.AddAll(cards);
            if (_shuffleDeck)
            {
                Player.Deck.Shuffle();
            }
        }
    }
}

using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Spells.Potions
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
            var cards = Player.Discard.GetCardsOfType(card => !card.Tags.Contains(Tag.Healing), _healingAmount);

            foreach (var card in cards)
            {
                Player.Discard.Remove(card);
                Player.Deck.Add(card);
            }

            if (_shuffleDeck)
            {
                Player.Deck.Shuffle();
            }
            else
            {
                Player.Deck.AdjustCardSpacing();
            }

            Player.Discard.AdjustCardSpacing();
        }
    }
}

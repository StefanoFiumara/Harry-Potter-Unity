using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public class HealingSpell : BaseSpell
    {
        [Header("Healing Spell Settings")]
        [SerializeField, UsedImplicitly]
        private int _healingAmount;

        [SerializeField, UsedImplicitly]
        private bool _shuffleDeckAfterHeal;

        protected override void SpellAction(List<BaseCard> targets)
        {
            var cards = Player.Discard.GetHealableCards().Take(_healingAmount);
            
            Player.Deck.AddAll(cards);
            if (_shuffleDeckAfterHeal)
            {
                Player.Deck.Shuffle();
            }
        }
    }
}

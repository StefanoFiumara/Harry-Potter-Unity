using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Utils;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class BiasedCommentary : BaseSpell
    {
        private bool _hasEffectedTriggered;

        protected override void SpellAction(List<BaseCard> targets)
        {
            _hasEffectedTriggered = false;
            Player.OnCardPlayedEvent += AddDamageToQuidditchCards;

            Player.OnNextTurnStart += () => Player.OnCardPlayedEvent -= AddDamageToQuidditchCards;

        }

        private void AddDamageToQuidditchCards(BaseCard card, List<BaseCard> targets)
        {
            if (card.IsQuidditchDamageCard() == false) return;
            if (_hasEffectedTriggered) return;

            _hasEffectedTriggered = true;

            ((IDamageSpell)card).DamageAmount += 5;
        }

        
    }
}
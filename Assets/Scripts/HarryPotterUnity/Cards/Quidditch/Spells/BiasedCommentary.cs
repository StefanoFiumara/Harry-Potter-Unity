using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class BiasedCommentary : BaseSpell
    {
        private bool _hasEffectedTriggered;

        //TODO: Test this
        protected override void SpellAction(List<BaseCard> targets)
        {
            _hasEffectedTriggered = false;
            Player.OnCardPlayed += AddDamageToQuidditchCards;

            Player.OnNextTurnStart += () => Player.OnCardPlayed -= AddDamageToQuidditchCards;

        }

        private void AddDamageToQuidditchCards(BaseCard card, List<BaseCard> targets)
        {
            if (!IsQuidditchDamageCard(card)) return;
            if (_hasEffectedTriggered) return;

            _hasEffectedTriggered = true;

            ((IDamageSpell)card).DamageAmount += 5;
        }

        //TODO: Look into turning this into an extension method
        private bool IsQuidditchDamageCard(BaseCard card)
        {
            return card is IDamageSpell &&
                   card.Classification == ClassificationTypes.Quidditch;
        }
    }
}
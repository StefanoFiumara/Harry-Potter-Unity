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
            this._hasEffectedTriggered = false;
            this.Player.OnCardPlayedEvent += this.AddDamageToQuidditchCards;

            this.Player.OnNextTurnStartEvent += () => this.Player.OnCardPlayedEvent -= this.AddDamageToQuidditchCards;

        }

        private void AddDamageToQuidditchCards(BaseCard card, List<BaseCard> targets)
        {
            if (card.IsQuidditchDamageCard() == false) return;
            if (this._hasEffectedTriggered) return;

            this._hasEffectedTriggered = true;

            ((IDamageSpell)card).DamageAmount += 5;
        }

        
    }
}
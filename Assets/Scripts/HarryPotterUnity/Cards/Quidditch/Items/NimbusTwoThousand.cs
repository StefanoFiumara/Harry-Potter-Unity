using System.Collections.Generic;
using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    public class NimbusTwoThousand : ItemLessonProvider
    {
        private bool _hasEffectedTriggered;
        protected int _damageAmount;

        protected override void Start()
        {
            base.Start();
            _damageAmount = 2;
        }
        public override void OnInPlayBeforeTurnAction()
        {
            _hasEffectedTriggered = false;
        }

        public override void OnEnterInPlayAction()
        {
            Player.OnCardPlayed += AddDamageToQuidditchCards;
        }

        private void AddDamageToQuidditchCards(BaseCard cardPlayed, List<BaseCard> targets)
        {
            if (!IsQuidditchDamageCard(cardPlayed)) return;
            if (_hasEffectedTriggered) return;

            _hasEffectedTriggered = true;
            //TODO: Add damage to the card's IDamageSpell property instead
            ((IDamageSpell) cardPlayed).DamageAmount += _damageAmount;
        }

        private bool IsQuidditchDamageCard(BaseCard card)
        {
            return card is IDamageSpell &&
                   card.Classification == ClassificationTypes.Quidditch;
        }

        public override void OnExitInPlayAction()
        {
            Player.OnCardPlayed -= AddDamageToQuidditchCards;
        }
    }
}
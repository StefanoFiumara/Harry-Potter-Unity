using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    [UsedImplicitly]
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
            Player.OnCardPlayedEvent += AddDamageToQuidditchCards;
        }

        private void AddDamageToQuidditchCards(BaseCard cardPlayed, List<BaseCard> targets)
        {
            if (!IsQuidditchDamageCard(cardPlayed)) return;
            if (_hasEffectedTriggered) return;

            _hasEffectedTriggered = true;
            Player.OppositePlayer.TakeDamage(this, _damageAmount);
        }

        private bool IsQuidditchDamageCard(BaseCard card)
        {
            return card is IDamageSpell &&
                   card.Classification == ClassificationTypes.Quidditch;
        }

        public override void OnExitInPlayAction()
        {
            Player.OnCardPlayedEvent -= AddDamageToQuidditchCards;
        }
    }
}
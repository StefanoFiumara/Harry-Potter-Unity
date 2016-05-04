using System.Collections.Generic;
using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Utils;

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
            if (cardPlayed.IsQuidditchDamageCard() == false) return;
            if (_hasEffectedTriggered) return;

            _hasEffectedTriggered = true;

            ((IDamageSpell) cardPlayed).DamageAmount += _damageAmount;
        }
        
        public override void OnExitInPlayAction()
        {
            Player.OnCardPlayed -= AddDamageToQuidditchCards;
        }
    }
}
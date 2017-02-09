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
            this._damageAmount = 2;
        }
        public override void OnInPlayBeforeTurnAction()
        {
            this._hasEffectedTriggered = false;
        }

        public override void OnEnterInPlayAction()
        {
            this.Player.OnCardPlayedEvent += this.AddDamageToQuidditchCards;
        }

        private void AddDamageToQuidditchCards(BaseCard cardPlayed, List<BaseCard> targets)
        {
            if (cardPlayed.IsQuidditchDamageCard() == false) return;
            if (this._hasEffectedTriggered) return;

            this._hasEffectedTriggered = true;

            ((IDamageSpell) cardPlayed).DamageAmount += this._damageAmount;
        }
        
        public override void OnExitInPlayAction()
        {
            this.Player.OnCardPlayedEvent -= this.AddDamageToQuidditchCards;
        }
    }
}
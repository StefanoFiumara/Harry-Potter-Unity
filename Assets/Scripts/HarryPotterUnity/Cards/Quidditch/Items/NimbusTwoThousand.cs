using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;
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
            if (!(cardPlayed is DirectDamageSpell) && !(cardPlayed is TargetedDamageSpell)) return;
            if (cardPlayed.Classification != ClassificationTypes.Quidditch) return;
            if (_hasEffectedTriggered) return;
            if (!(targets.First() is BaseCharacter)) return;

            _hasEffectedTriggered = true;
            Player.OppositePlayer.TakeDamage(_damageAmount);
        }

        public override void OnExitInPlayAction()
        {
            Player.OnCardPlayedEvent -= AddDamageToQuidditchCards;
        }
    }
}
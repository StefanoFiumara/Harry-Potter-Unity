using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    [UsedImplicitly]
    public class NimbusTwoThousand : BaseItem
    {
        public override void OnEnterInPlayAction()
        {
            Player.OnCardPlayedEvent += AddDamageToQuidditchCards;
        }

        private void AddDamageToQuidditchCards(BaseCard cardPlayed)
        {
            if (cardPlayed is DirectDamageSpell || cardPlayed is TargetedDamageSpell)
            {
                if (cardPlayed.Classification == ClassificationTypes.Quidditch)
                {
                    Player.OppositePlayer.TakeDamage(2);
                }
            }
        }

        public override void OnExitInPlayAction()
        {
            Player.OnCardPlayedEvent -= AddDamageToQuidditchCards;
        }
    }
}
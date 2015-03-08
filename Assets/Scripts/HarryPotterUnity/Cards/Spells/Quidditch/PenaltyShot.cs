using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Spells.Quidditch
{
    [UsedImplicitly]
    public class PenaltyShot : GenericSpell 
    {
        protected override void OnPlayAction()
        {
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
            Player.Deck.DrawCard();
        }
    }
}

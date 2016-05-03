using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class PracticeMatch : BaseMatch
    {
        public override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            winner.Deck.DrawCard();
            winner.Deck.DrawCard();
            winner.Deck.DrawCard();
            winner.Deck.DrawCard();
        }
    }
}

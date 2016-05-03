using System.Linq;
using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class SlytherinMatch : BaseMatch
    {
        public override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            var cards = winner.Discard.NonHealingCards.Take(15);

            winner.Deck.AddAll(cards);
            winner.Deck.Shuffle();
        }
    }
}
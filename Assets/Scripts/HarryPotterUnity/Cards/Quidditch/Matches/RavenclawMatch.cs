using System.Linq;
using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class RavenclawMatch : BaseMatch
    {
        public override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            var lessons = winner.Deck.Lessons.Take(2);

            winner.InPlay.AddAll(lessons);
            winner.Deck.Shuffle();
        }
    }
}
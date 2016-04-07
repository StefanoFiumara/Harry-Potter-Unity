using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class ThreeMonthLongMatch : BaseMatch
    {
        protected override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            loser.TakeDamage(this, 15);
        }
    }
}
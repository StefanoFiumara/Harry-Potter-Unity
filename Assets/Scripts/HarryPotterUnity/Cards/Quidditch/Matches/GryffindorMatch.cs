using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class GryffindorMatch : BaseMatch
    {
        protected override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            winner.OnNextTurnStart += () => winner.AddActions(2);
        }
    }
}